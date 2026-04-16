import serial
import pika
import json
from datetime import datetime, UTC

# Serial connection (sensor)
ser = serial.Serial('COM5', 9600)

# RabbitMQ connection
connection = pika.BlockingConnection(
    pika.ConnectionParameters('10.133.51.104')
)
channel = connection.channel()
channel.queue_declare(queue='sensor')

print("Producer started...")

while True:
    line = ser.readline().decode().strip()

    try:
        value = int(line)
        timestamp = datetime.now(UTC)

        message = json.dumps({
            "timestamp": timestamp.isoformat(),
            "value": value
        })

        channel.basic_publish(
            exchange='',
            routing_key='sensor',
            body=message
        )

        print("Sent:", message)

    except Exception as e:
        print("Error:", e)