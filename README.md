<div align="center">

# FITGYM — Gym Management System

### A full-featured, professional gym management platform built with ASP.NET Core 10.0

[![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-10.0-512BD4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![Entity Framework](https://img.shields.io/badge/EF_Core-10.0-512BD4?style=for-the-badge&logo=entityframework)](https://learn.microsoft.com/en-us/ef/)
[![Stripe](https://img.shields.io/badge/Stripe-Payments-635BFF?style=for-the-badge&logo=stripe)](https://stripe.com/)
[![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)](LICENSE.txt)

</div>

---

## Table of Contents

- [About](#about)
- [Demo](#demo)
- [Screenshots](#screenshots)
- [Architecture](#architecture)
- [Tech Stack](#tech-stack)
- [Features](#features)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Roles and Permissions](#roles-and-permissions)
- [Payment Integration](#payment-integration)
- [Key Endpoints](#key-endpoints)
- [Contributing](#contributing)
- [License](#license)

---

<a id="about"></a>

## About

**FITGYM** is a comprehensive gym management system designed for real-world fitness centers. It provides:

- **For Admins:** Full control over members, trainers, sessions, plans, memberships, discounts, bookings, check-ins, body measurements, and audit logs.
- **For Members:** A personal dashboard to manage their profile, view memberships, book sessions, track progress, and pay via **Stripe**.
- **For Guests:** Easy registration with role-based access control ensuring members never see admin functionality.

**Admin Credentials (seeded):**
- Email: `superadmin@example.com` | Password: `ChangeMe_Str0ngP@ss!`
- Email: `admin@example.com` | Password: `ChangeMe_Str0ngP@ss!`

---

<a id="demo"></a>

## Demo

🔗 **Live Demo:** [https://fitgym.runasp.net/](https://fitgym.runasp.net/)

**Admin Credentials (seeded):**
- Email: `superadmin@example.com` | Password: `ChangeMe_Str0ngP@ss!`

---

<a id="screenshots"></a>

## Screenshots

### 🏠 Public Landing Page

<p align="center">
  <img src="screenshots/01-Home-Hero.png" width="900" alt="Home Hero Section"/>
</p>

<p align="center">
  <img src="screenshots/02-Home-Services-Stats.png" width="900" alt="Services and Stats Section"/>
</p>

<p align="center">
  <img src="screenshots/03-Home-CTA-Footer.png" width="900" alt="CTA and Footer Section"/>
</p>

### 🔐 Authentication

<p align="center">
  <img src="screenshots/04-Login-Page.png" width="900" alt="Login Page"/>
</p>

### 📊 Admin Dashboard

<p align="center">
  <img src="screenshots/05-Admin-Home-LoggedIn.png" width="900" alt="Admin Home After Login"/>
</p>

<p align="center">
  <img src="screenshots/06-Admin-Dashboard-Overview.png" width="900" alt="Dashboard Overview with Analytics"/>
</p>

### 👥 People Management

<p align="center">
  <img src="screenshots/07-People-Dropdown-Menu.png" width="900" alt="People Dropdown Menu"/>
</p>

<p align="center">
  <img src="screenshots/08-Members-Management.png" width="900" alt="Members Management"/>
</p>

<p align="center">
  <img src="screenshots/09-Trainers-Management.png" width="900" alt="Trainers Management"/>
</p>

<p align="center">
  <img src="screenshots/10-CheckIn-Today.png" width="900" alt="Today's Check-ins"/>
</p>

<p align="center">
  <img src="screenshots/11-Body-Measurements.png" width="900" alt="Body Measurements Tracking"/>
</p>

### 📅 Content Management

<p align="center">
  <img src="screenshots/12-Content-Dropdown-Sessions.png" width="900" alt="Content Dropdown - Sessions"/>
</p>

<p align="center">
  <img src="screenshots/13-Sessions-Schedule.png" width="900" alt="Sessions Schedule"/>
</p>

<p align="center">
  <img src="screenshots/14-Membership-Plans.png" width="900" alt="Membership Plans"/>
</p>

### 💳 Business Management

<p align="center">
  <img src="screenshots/15-Memberships-Management.png" width="900" alt="Memberships Management"/>
</p>

<p align="center">
  <img src="screenshots/16-Discounts-PromoCodes.png" width="900" alt="Discounts and Promo Codes"/>
</p>

<p align="center">
  <img src="screenshots/17-Bookings-Management.png" width="900" alt="Bookings Management"/>
</p>

<p align="center">
  <img src="screenshots/18-Audit-Log.png" width="900" alt="Audit Log"/>
</p>

### 🌍 Localization

<p align="center">
  <img src="screenshots/19-Arabic-Localization-RTL.png" width="900" alt="Arabic RTL Localization"/>
</p>

---

<a id="architecture"></a>

## Architecture

The project follows **Clean Architecture** with three distinct layers:

```
+-------------------------------------------------+
|                  PL (Presentation)               |
|          MVC Controllers + Razor Views           |
+-------------------------------------------------+
|                  BLL (Business Logic)            |
|     Services  |  ViewModels  |  AutoMapper       |
+-------------------------------------------------+
|                  DAL (Data Access)               |
|  EF Core  |  Repositories  |  Unit of Work      |
+-------------------------------------------------+
```

**Design Patterns Used:**
- **Repository Pattern** - Generic repository with `IGenericRepositories<T>` for CRUD operations
- **Unit of Work** - Transaction management via `IUnitOfWork`
- **Service Layer** - Business logic encapsulated in services with interface segregation
- **Result Pattern** - Typed `Result<T>` for clean error handling without exceptions

---

<a id="tech-stack"></a>

## Tech Stack

| Layer | Technology |
|-------|-----------|
| **Framework** | ASP.NET Core 10.0 MVC |
| **ORM** | Entity Framework Core 10 |
| **Database** | SQL Server |
| **Identity** | ASP.NET Core Identity (Cookie + JWT Bearer) |
| **Payment** | Stripe Checkout (Stripe.net v52) |
| **Mapping** | AutoMapper v16 |
| **QR Codes** | QRCoder v1.8 |
| **Charts** | Chart.js |
| **Frontend** | Bootstrap 5.3, Inter Font, Bootstrap Icons |
| **Localization** | ASP.NET Core Localization (English + Arabic) |
| **Security** | Rate Limiting, CSRF Protection, XSS Headers |

---

<a id="features"></a>

## Features

### Authentication and Authorization

- User Registration with auto Member profile creation
- Login with rate limiting (5 attempts/min per IP)
- Forgot Password with email reset links
- Reset Password flow
- Role-based access: **SuperAdmin**, **Admin**, **Member**
- Cookie + JWT Bearer authentication
- Access Denied page with clear messaging

### Member Management

- Full CRUD with photo upload
- Health records (height, weight, blood type, notes)
- Address management
- Member search and filtering
- Profile self-editing for members

### Trainer Management

- CRUD operations with specialization tracking
- Trainer selection for sessions

### Session Management

- Create, edit, view sessions with date/time/capacity
- Category-based organization
- Trainer assignment
- Session scheduling with attendees view

### Membership and Plans

- Plan management (name, price, duration, active/inactive)
- Membership lifecycle: **Pending**, **Active**, **Expired**, **Cancelled**
- Discount code system with validation
- Membership renewal
- Payment recording with multiple methods

### Stripe Payment Integration

- Stripe Checkout Session redirect flow
- Secure hosted payment page (PCI compliant)
- Webhook verification for payment confirmation
- Pending membership auto-activation on payment success
- Cancel flow with membership cleanup

### Dashboard

- Analytics overview (total members, revenue, active memberships)
- Plan distribution charts
- Recent activity

### Check-In System

- QR code generation per member
- QR scan endpoint for check-in
- Today's check-in tracking

### Body Measurements

- Track member progress (weight, height, etc.)
- Measurement history

### Notifications

- Bell icon with unread count
- Notification center

### Audit Log

- Full audit trail of admin actions
- Logged user, action, entity, timestamp

### Localization

- English (en-US) and Arabic (eg-EG) support
- RTL layout for Arabic
- Culture switcher in navbar

### Professional UI/UX

- Dark-only theme with indigo accent (#6366f1)
- Inter font family
- CSS diamond brand icon (no image logo)
- Scroll-triggered navbar with blur effect
- Grouped admin dropdown menus with icons
- Animated hero section (pulse rings, floating icon)
- Scroll reveal animations (Intersection Observer)
- Animated counters
- Marquee services bar
- Hover micro-interactions on all cards
- Responsive design (mobile-first)
- Shared partials system (_Alerts, _PageHeader, _Pagination, etc.)

---

<a id="project-structure"></a>

## Project Structure

```
GymManagementSystem/
|-- GymManagement/                          # Presentation Layer (PL)
|   |-- Controllers/                        # 15+ MVC Controllers
|   |   |-- AccountController.cs            # Login, Register, ForgotPassword, ResetPassword
|   |   |-- MembersController.cs            # Admin: Member CRUD
|   |   |-- MemberController.cs             # Member portal: Profile, Membership, Bookings, Subscribe
|   |   |-- TrainersController.cs           # Trainer CRUD
|   |   |-- SessionsController.cs           # Session management
|   |   |-- PlansController.cs              # Plan management
|   |   |-- MembershipsController.cs        # Membership management + payments
|   |   |-- DiscountsController.cs          # Discount management
|   |   |-- BookingsController.cs           # Booking management
|   |   |-- CheckInController.cs            # QR scan + today's check-ins
|   |   |-- MeasurementsController.cs       # Body measurements
|   |   |-- DashboardController.cs          # Analytics dashboard
|   |   |-- AuditController.cs              # Audit logs
|   |   |-- NotificationsController.cs      # Notifications
|   |   +-- CultureController.cs            # Language switching
|   |-- Models/                             # PL-specific models
|   |-- ViewComponents/                     # NotificationBell component
|   |-- Views/                              # Razor views (~40 pages)
|   |-- SharedResources/                    # Localization (.resx files)
|   |-- wwwroot/                            # Static files (CSS, JS, images)
|   |-- Program.cs                          # Application configuration and DI
|   +-- appsettings.json                    # Configuration
|
|-- GymManagementSystem.BLL/               # Business Logic Layer
|   |-- Services/
|   |   |-- Interfaces/                     # 14 service interfaces
|   |   +-- Classes/                        # Service implementations
|   |-- ViewModels/                         # View models for all entities
|   |-- Common/                             # Result pattern, enums
|   +-- MappingProfile.cs                   # AutoMapper configuration
|
|-- GymManagementSystem.DAL/               # Data Access Layer
|   |-- Models/                             # Entity models
|   |-- Configurations/                     # EF Core entity configurations
|   |-- Repositories/
|   |   |-- Interfaces/                     # Generic + specific repository interfaces
|   |   +-- Classes/                        # Repository implementations
|   |-- DataSeeding/                        # Identity + data seeding
|   |-- Migrations/                         # EF Core migrations
|   |-- DbContexts/                         # GymDbContext
|   +-- UnitOfWork.cs                       # Unit of Work implementation
|
|-- baseline.sql                            # Database baseline script
+-- GymManagementSystem.slnx                # Solution file
```

---

<a id="getting-started"></a>

## Getting Started

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server) (LocalDB, Express, or full)
- [Stripe Account](https://stripe.com/) (for payment features)

### Installation

```bash
# Clone the repository
git clone https://github.com/EngMohamedNowar/GymManagementSystem_MVC.git
cd GymManagementSystem

# Restore packages
dotnet restore

# Update connection string in GymManagement/appsettings.json
# "DefaultConnection": "Server=.; Database=GymManagementSystem;Trusted_Connection=True;TrustServerCertificate=True;"

# Run migrations and start
cd GymManagement
dotnet run --urls "http://localhost:5050"
```

The app will automatically:
1. Apply pending migrations
2. Seed roles (SuperAdmin, Admin, Member)
3. Seed admin accounts
4. Seed sample gym data

---

<a id="configuration"></a>

## Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.; Database=GymManagementSystem;Trusted_Connection=True;"
  },
  "Jwt": {
    "Key": "YourSecretKeyHere",
    "Issuer": "GymManagementSystem",
    "Audience": "GymManagementSystem"
  },
  "Stripe": {
    "SecretKey": "sk_test_...",
    "PublishableKey": "pk_test_...",
    "WebhookSecret": "whsec_..."
  },
  "AdminSeed": {
    "Password": "ChangeMe_Str0ngP@ss!",
    "SuperAdminEmail": "superadmin@example.com",
    "AdminEmail": "admin@example.com"
  }
}
```

### Stripe Webhook (Development)

```bash
stripe listen --forward-to localhost:5050/Member/StripeWebhook
```

---

<a id="roles-and-permissions"></a>

## Roles and Permissions

| Feature | SuperAdmin | Admin | Member | Guest |
|---------|:----------:|:-----:|:------:|:-----:|
| Dashboard | Yes | Yes | -- | -- |
| Members CRUD | Yes | -- | -- | -- |
| Trainers CRUD | Yes | -- | -- | -- |
| Sessions CRUD | Yes | -- | -- | -- |
| Plans CRUD | Yes | -- | -- | -- |
| Memberships | Yes | -- | -- | -- |
| Discounts | Yes | Yes | -- | -- |
| Bookings | Yes | -- | -- | -- |
| Check-In | Yes | Yes | -- | -- |
| Measurements | Yes | Yes | Yes | -- |
| Audit Logs | Yes | -- | -- | -- |
| Member Portal | Yes | Yes | Yes | -- |
| Subscribe + Pay | -- | -- | Yes | -- |
| Home Page | Yes | Yes | Yes | Yes |
| Register | -- | -- | -- | Yes |
| Login | Yes | Yes | Yes | Yes |

---

<a id="payment-integration"></a>

## Payment Integration (Stripe)

### Flow

```
Member selects plan -> Clicks "Pay with Stripe"
        |
Creates Pending membership in database
        |
Redirects to Stripe Checkout (hosted, PCI-compliant)
        |
Member pays with Visa/Mastercard on Stripe
        |
Stripe redirects to /Member/PaySuccess
        |
Stripe Webhook confirms payment
        |
Membership status: Pending -> Active
Payment record created with Stripe reference
```

### Endpoints

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/Member/Subscribe` | GET | Plan selection page |
| `/Member/Subscribe` | POST | Create pending membership + redirect to Stripe |
| `/Member/PaySuccess` | GET | Success page after Stripe redirect |
| `/Member/PayCancel` | GET | Cancel page + cleanup pending membership |
| `/Member/StripeWebhook` | POST | Stripe webhook (payment confirmation) |

---

<a id="key-endpoints"></a>

## Key Endpoints

### Public

| Route | Description |
|-------|-------------|
| `GET /` | Landing page with hero, features, stats |
| `GET /Account/Register` | User registration |
| `GET /Account/Login` | Login page |
| `GET /Account/ForgotPassword` | Password reset request |

### Admin (SuperAdmin)

| Route | Description |
|-------|-------------|
| `GET /Dashboard` | Analytics dashboard |
| `GET /Members` | Member management |
| `GET /Trainers` | Trainer management |
| `GET /Sessions` | Session management |
| `GET /Plans` | Plan management |
| `GET /Memberships` | Membership management |
| `GET /Discounts` | Discount management |
| `GET /Bookings` | Booking management |
| `GET /CheckIn/Today` | Today's check-ins |
| `GET /Audit` | Audit trail |

### Member Portal

| Route | Description |
|-------|-------------|
| `GET /Member` | Personal dashboard |
| `GET /Member/MyMembership` | Membership + payments |
| `GET /Member/MyBookings` | Session bookings |
| `GET /Member/MyProfile` | Edit profile |
| `GET /Member/MyQR` | QR code for check-in |
| `GET /Member/Subscribe` | Subscribe with Stripe |

---

<a id="contributing"></a>

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Branch Strategy

- `main` - Production-ready code
- `develop` - Active development branch
- Feature branches from `develop`

---

<a id="license"></a>

## License

This project is licensed under the MIT License. See the [LICENSE.txt](LICENSE.txt) file for details.

---

<div align="center">

**Built with .NET 10.0 and ASP.NET Core**

[Back to Top](#table-of-contents)

</div>
