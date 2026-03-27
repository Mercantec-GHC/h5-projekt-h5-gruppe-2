import serial
import pika
import time

# Serial (Arduino)
ser = serial.Serial('COM5', 9600)  

# RabbitMQ connection
connection = pika.BlockingConnection(
    pika.ConnectionParameters('localhost')
)
print("Connected to RabbitMQ")
channel = connection.channel()

channel.queue_declare(queue='sensor_data')

while True:
    line = ser.readline().decode().strip()

    try:
        value = int(line)

        message = f"{time.time()},{value}"
        channel.basic_publish(
            exchange='',
            routing_key='sensor_data',
            body=message
        )

        print("Sent:", message)

    except Exception as e:
        print("Error:", e)