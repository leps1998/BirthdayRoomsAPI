# BirthdayRooms API

API para la gestión de reservas de salas de cumpleaños. Implementada con ASP.NET Core, validaciones con FluentValidation y pruebas unitarias para la lógica de negocio.

---

## Tecnologías utilizadas

- ASP.NET Core 8
- Entity Framework Core
- SQL Server (InMemory para testing)
- AutoMapper
- FluentValidation
- Swagger (OpenAPI)
- xUnit (Tests)

---

## Estructura del Proyecto

```
BirthdayRoomsBackend
│
├── Bookings
│   ├── Exceptions
│   │   ├── BookingDurationException.cs
│   │   ├── InvalidBookingTimeException.cs
│   │   └── OverlappingBookingException.cs
│   ├── BookingRules.cs
│   ├── BookingService.cs
│   └── IBookingService.cs
│
├── Controllers
│   └── BookingController.cs
│
├── Data
│   ├── Seed
│   │   └── DatabaseSeeder.cs
│   └── AppDbContext.cs
│
├── DTOs
│   ├── BookingRequestDTO.cs
│   └── Validators
│       └── BookingRequestValidator.cs
│
├── Models
│   ├── Booking.cs
│   └── Room.cs
│
├── Services
│   └── BookingServiceTests.cs
│
├── appsettings.json
├── BirthdayRoomsBackend.http
├── Dockerfile
├── Program.cs
└── README.md
```

---

## Endpoints

### 1. Crear una reserva

- **POST** `/api/bookings`
- **Descripción:** Crea una nueva reserva para una sala.
- **Body:**
```json
{
  "roomId": 1,
  "startTime": "2025-07-26T14:00:00",
  "endTime": "2025-07-26T16:00:00"
}
```
- **Respuestas:**
  - `201 Created` – Reserva creada exitosamente.
  - `400 Bad Request` – Horario inválido o duración incorrecta.
  - `409 Conflict` – Conflicto con otra reserva existente.

---

### 2. Obtener todas las reservas

- **GET** `/api/bookings`
- **Descripción:** Devuelve todas las reservas registradas.
- **Respuestas:**
  - `200 OK` – Lista de reservas.

---

### 3. Obtener una reserva por ID

- **GET** `/api/bookings/{id}`
- **Descripción:** Devuelve una reserva específica.
- **Respuestas:**
  - `200 OK` – Reserva encontrada.
  - `404 Not Found` – No existe una reserva con ese ID.

---

### 4. Obtener reservas por fecha

- **GET** `/api/bookings/by-date/{date}`
- **Descripción:** Devuelve todas las reservas en una fecha específica.
- **Parámetro:** `date` en formato `yyyy-MM-dd`.
- **Ejemplo:** `/api/bookings/by-date/2025-07-26`
- **Respuestas:**
  - `200 OK` – Reservas en esa fecha.
  - `400 Bad Request` – Formato de fecha incorrecto.

---

### 5. Actualizar una reserva

- **PUT** `/api/bookings/{id}`
- **Descripción:** Actualiza una reserva existente.
- **Body:**
```json
{
  "roomId": 1,
  "startTime": "2025-07-26T17:00:00",
  "endTime": "2025-07-26T19:00:00"
}
```
- **Respuestas:**
  - `200 OK` – Reserva actualizada.
  - `404 Not Found` – Reserva no encontrada.
  - `400 Bad Request` – Error de validación.
  - `409 Conflict` – Conflicto con otra reserva existente.

---

### 6. Eliminar una reserva

- **DELETE** `/api/bookings/{id}`
- **Descripción:** Elimina una reserva específica.
- **Respuestas:**
  - `204 No Content` – Eliminación exitosa.
  - `404 Not Found` – Reserva no encontrada.

---

## Tests

`BookingServiceTests.cs` contiene pruebas unitarias para:

- Verificar duración válida.
- Evitar superposición de reservas.
- Comportamiento esperado del servicio.

---

## Cómo Ejecutar

```bash
dotnet restore
dotnet run --project BirthdayRoomsBackend
```

---

## Docker

```bash
docker build -t birthdayrooms-api .
docker run -p 5000:80 birthdayrooms-api
```

---

## Autor

Desarrollado por [Leonel Perez](https://github.com/leps1998)
