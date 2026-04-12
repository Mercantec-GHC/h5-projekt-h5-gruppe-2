import serial
import json
import paho.mqtt.client as mqtt
from datetime import datetime, UTC

# Serial (Arduino)
ser = serial.Serial('COM5', 9600)

# MQTT client
client = mqtt.Client()

# Connect to HiveMQ (public broker for testing)
client.connect("broker.hivemq.com", 1883, 60)

topic = "sensor/data"

print("MQTT Producer started...")

while True:
    line = ser.readline().decode().strip()

    try:
        value = int(line)
        timestamp = datetime.now(UTC)

        message = json.dumps({
            "timestamp": timestamp.isoformat(),
            "value": value
        })

        client.publish(topic, message)

        print("Sent:", message)

    except Exception as e:
        print("Error:", e)