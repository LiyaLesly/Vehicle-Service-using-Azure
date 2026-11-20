# Vehicle Service

A vehicle service booking system built with ASP.NET Core (.NET 8). Customers register vehicles, book services and view bills. When a booking is created, a confirmation email is sent through **Azure Service Bus** and **Azure Communication Services (ACS) Email**.

This project was migrated from an AWS-style messaging setup (SQS/SES) to Azure.

## Architecture

```
Customer -> Vehicle_Service (MVC web app)
              |  1. saves booking to SQL Server
              |  2. POST /api/BookingNotification  (Bearer JWT)
              v
        VehicleServiceAPI  -- publishes message -->  Azure Service Bus queue
                                                          |
                                                          v
                                              VehicleService.Worker
                                                          |
                                                          v
                                          Azure Communication Services Email
                                                          |
                                                          v
                                                 Customer's inbox
```

| Project | Description |
|---|---|
| `Vehicle_Service` | ASP.NET Core MVC web app: login, vehicles, bookings, bills |
| `VehicleServiceAPI` | Web API: JWT login and the booking notification endpoint |
| `VehicleService.Worker` | Background worker: reads the queue and sends email |
| `VehicleService.Messaging` | Shared library: message model, publisher, email service |

## Tech stack

- .NET 8, ASP.NET Core MVC and Web API
- SQL Server (local SQL Express)
- JWT authentication with role-based authorization (`Customer`, `ServiceManager`)
- Azure Service Bus (Basic tier, queue)
- Azure Communication Services Email

## Prerequisites

- Visual Studio 2022 or later, or the .NET 8 SDK
- SQL Server / SQL Express with the `Vehicle_mvc_core` database
- An Azure subscription with:
  - a Service Bus namespace and a queue named `booking-queue`
  - a Communication Service connected to an Email Communication Service and domain

## Configuration

Secrets are **not** stored in this repository. Set them with user secrets
(Visual Studio: right-click the project, then **Manage User Secrets**, or `dotnet user-secrets set`).

**VehicleServiceAPI**

```json
{
  "ServiceBus": {
    "ConnectionString": "<Service Bus queue connection string>",
    "QueueName": "booking-queue"
  },
  "Jwt": {
    "Key": "<long random signing key>",
    "Issuer": "<issuer>",
    "Audience": "<audience>",
    "ExpiryMinutes": "30"
  },
  "ConnectionStrings": {
    "DefaultConnection": "<SQL Server connection string>"
  }
}
```

**VehicleService.Worker**

```json
{
  "ServiceBus": { "ConnectionString": "<Service Bus queue connection string>" },
  "Acs": {
    "ConnectionString": "<ACS connection string>",
    "Sender": "DoNotReply@<your-domain>.azurecomm.net"
  }
}
```

The Worker's `appsettings.json` holds the non-secret values: `ServiceBus:QueueName = booking-queue` and `Email:UseAcs = true`.

**Vehicle_Service (web)**

```json
"ApiSettings": { "BaseUrl": "https://localhost:7097" }
```

Set `Email:UseAcs` to `false` in the Worker to log emails instead of sending them (useful for local testing).

## Running locally

1. Add the secrets above.
2. In Visual Studio, right-click the solution, choose **Configure Startup Projects**, then **Multiple startup projects**, and set the API, the web app and the Worker to **Start**.
3. Press **Start**.
4. Check the consoles:
   - API: `Now listening on: https://localhost:7097`
   - Worker: `Listening on queue booking-queue`
5. Log in as a customer in the web app and create a booking.
6. Expected logs:
   - API: `Published booking N`
   - Worker: `Received message`, `Email submitted to ...`, `Processed booking N`
7. Check the customer's inbox (and spam folder) for **Service booking #N received**.

You can also test the API alone at `https://localhost:7097/swagger`: call `POST /api/Login`, click **Authorize** with the token, then call `POST /api/BookingNotification` with a customer token.

## How messages are handled

- Invalid JSON goes to the dead-letter queue.
- Processing failures are retried up to 3 times, then dead-lettered.
- A messaging failure never blocks a booking from being saved.

## Known limitations and next steps

- The database connection string is hardcoded in the DB classes; move it to configuration and Azure SQL for deployment.
- Passwords are stored as plain text; hash them.
- The booking form trusts the posted customer ID; read it from the session instead.
- Emails from the free `azurecomm.net` domain may land in spam; use a custom domain for production.
- Deployment target: Azure App Service (API, web app, and Worker with Always On).

## Security

Never commit connection strings, access keys, or the JWT signing key. If a key is exposed, regenerate it in the Azure portal immediately.
