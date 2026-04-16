import pika
import psycopg2
import json

# PostgreSQL connection
conn = psycopg2.connect(
    host="10.133.51.104",
    database="sensor",
    user="kasper",
    password="H5kode"
)
cur = conn.cursor()

# RabbitMQ connection
connection = pika.BlockingConnection(
    pika.ConnectionParameters('10.133.51.104')
)
channel = connection.channel()
channel.queue_declare(queue='sensor')

print("Consumer waiting for messages...")

def callback(ch, method, properties, body):
    try:
        data = json.loads(body)

        timestamp = data["timestamp"]
        value = data["value"]

        cur.execute(
            "INSERT INTO measurements (timestamp, value) VALUES (%s, %s)",
            (timestamp, value)
        )
        conn.commit()

        print("Saved to DB:", data)

        # Acknowledge message (IMPORTANT)
        ch.basic_ack(delivery_tag=method.delivery_tag)

    except Exception as e:
        print("Error:", e)

        conn.rollback()  # rollback in case of error

# Listen for messages
channel.basic_consume(
    queue='sensor',
    on_message_callback=callback,
    auto_ack=False  # important!
)

channel.start_consuming()