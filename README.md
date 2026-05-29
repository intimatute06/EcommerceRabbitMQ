# Sistema de E-commerce con RabbitMQ

Practica de arquitectura de software que implementa un sistema de mensajeria asincrona usando RabbitMQ aplicado a un caso real de e-commerce.

## Autores

- Inti Matute
- Jose Jauregui

## Docente

Monica Sanchez

## Descripcion

Cuando un cliente realiza un pedido, el sistema notifica automaticamente a tres servicios de forma independiente y simultanea:

- Servicio de Inventario: descuenta el stock del producto vendido
- Servicio de Logistica: prepara el envio del pedido
- Servicio de Notificaciones: envia confirmacion al cliente

## Tecnologias

- C# con .NET 10
- RabbitMQ 3.12
- Docker Desktop

## Exchanges configurados

- ecommerce.direct: enruta mensajes por coincidencia exacta de routing key
- ecommerce.fanout: enruta mensajes a todas las colas sin importar la routing key
- ecommerce.topic: enruta mensajes usando patrones con comodines

## Colas configuradas

- ecommerce.inventario: recibe pedidos para descontar stock
- ecommerce.logistica: recibe pedidos para preparar envios
- ecommerce.notificaciones: recibe eventos para notificar al cliente

## Estructura del proyecto

- PedidoEvent.cs: modelo del mensaje con los datos del pedido
- RabbitMQProducer.cs: publica mensajes en los tres exchanges
- RabbitMQConsumer.cs: consumidor del servicio de notificaciones
- RabbitMQConsumerInventario.cs: consumidor del servicio de inventario
- Program.cs: menu principal del sistema

## Requisitos previos

- Docker Desktop instalado y corriendo
- .NET 10 instalado

## Instalacion

1. Clonar el repositorio

git clone https://github.com/intimatute06/EcommerceRabbitMQ.git

2. Entrar a la carpeta del proyecto

cd EcommerceRabbitMQ

3. Levantar RabbitMQ con Docker

docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 -v rabbitmq_data:/var/lib/rabbitmq rabbitmq:3.12-management

4. Verificar que RabbitMQ esta corriendo en http://localhost:15672 con usuario guest y contrasena guest

5. Restaurar dependencias

dotnet restore

## Ejecucion

El sistema tiene tres modos de ejecucion. Se necesitan tres terminales abiertas.

Terminal 1 - Consumidor Inventario

cd EcommerceRabbitMQ
dotnet run
Elegir opcion 3

Terminal 2 - Consumidor Notificaciones

cd EcommerceRabbitMQ
.\bin\Debug\net10.0\EcommerceRabbitMQ.exe
Elegir opcion 2

Terminal 3 - Productor

cd EcommerceRabbitMQ
.\bin\Debug\net10.0\EcommerceRabbitMQ.exe
Elegir opcion 1
Ingresar los datos del pedido solicitados

## Configuracion de RabbitMQ

Antes de ejecutar el proyecto se debe configurar RabbitMQ con los siguientes pasos:

1. Crear Virtual Host llamado ecommerce en Admin - Virtual Hosts
2. Dar permisos al usuario guest ejecutando en la terminal del contenedor:
   rabbitmqctl set_permissions -p ecommerce guest ".*" ".*" ".*"
3. Crear los tres exchanges en la pestana Exchanges
4. Crear las tres colas en la pestana Queues and Streams
5. Configurar los bindings en cada cola

## Flujo del mensaje

Cuando el productor envia un pedido ocurre lo siguiente:

- Direct con routing key pedido.nuevo llega a inventario y logistica
- Fanout llega a logistica y notificaciones
- Topic con routing key pedido.creado llega a inventario y notificaciones

Cada cola recibe 2 mensajes procesados de forma independiente.