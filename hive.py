import paho.mqtt.client as mqtt
import psycopg2
import json

# PostgreSQL
conn = psycopg2.connect(
    host="10.133.51.104",
    port=5433,
    database="sensor",
    user="kasper",
    password="H5kode"
)
cur = conn.cursor()

topic = "sensor/data"

def on_message(client, userdata, msg):
    try:
        data = json.loads(msg.payload.decode())

        timestamp = data["timestamp"]
        value = data["value"]

        cur.execute(
            "INSERT INTO measurements (timestamp, value) VALUES (%s, %s)",
            (timestamp, value)
        )
        conn.commit()

        print("Saved to DB:", data)

    except Exception as e:
        print("Error:", e)
        conn.rollback()

# MQTT client
client = mqtt.Client()
client.connect("broker.hivemq.com", 1883, 60)

client.subscribe(topic)
client.on_message = on_message

print("MQTT Consumer listening...")

client.loop_forever()