
# VendingManager

A modern, full-stack solution for managing vending machine fleets in real-time.
--
## 📖 About The Project

**VendingManager** is a comprehensive dashboard designed to help administrators monitor and maintain a network of vending machines efficiently. Unlike traditional static dashboards, this application leverages **SignalR** for real-time updates, allowing immediate reaction to sales, stock depletions, or machine errors as they happen.

The project is built with a **Headless Architecture** mindset:
* **Backend:** Robust ASP.NET Core API hosted on **Azure Container Apps**.
* **Frontend:** Modern React Client (SPA) hosted on **Vercel**.

It was designed with **Mobile First** principles, ensuring field technicians can use the system comfortably on their smartphones while restocking machines or troubleshooting errors on-site.

---

## 🌐 Live Demo
The application is deployed and publicly accessible!
| Component | URL | Hosting Provider |
| :--- | :--- | :--- |
| **Backend** | https://vendingmanager-app-bz123.purpleflower-915564b1.polandcentral.azurecontainerapps.io/ | Azure Container Apps |
| **Frontend** | https://vending-manager-client.vercel.app/ | Vercel |

**To log in, use the following details:**

**Email:** ```admin@vending.com```

**Password:** ```Admin123!```

Note: The backend runs on a serverless plan, so the first request might take a few seconds to wake up the container.



## 🚀 Key Features

### 🔌 Secure IoT Machine Interface
* **API Key Authentication:** Each machine authenticates using a unique key (secured via ApiKeyAuthFilter), strictly preventing unauthorized access.
* **Instant Sales Sync:** Sales data ```/sale``` is transmitted instantly, automatically updating inventory levels in the database in real-time.
* **Remote Error Reporting:** Machines autonomously report technical faults (e.g., coin mechanism failure) directly to the administration panel.

### 📊 Interactive Dashboard
* **Real-time updates:** Sales and errors pop up instantly via WebSockets (SignalR) without refreshing the page.
* **Financial Analysis:** Interactive charts showing revenue trends over time.
* **Live Map:** Leaflet integration showing machine locations and status (Online/Offline) with custom, dynamic markers.

### 📦 Smart Inventory & Restocking
* **Route Optimization:** Automatically generates a "Service Route" for machines needing attention.
* **Shopping List:** Aggregates all missing products across the fleet into a single purchasing list.
* **Low Stock Alerts:** Visual indicators for critically low inventory levels.

### 🛡️ Management & Security
* **Role-Based Access Control (RBAC):** A robust permission system ```UserManagementController``` that strictly distinguishes between Administrators and Service Technicians/Managers.
* **Maintenance Mode:** A global switch ```MaintenanceController``` allowing administrators to safely take the system or specific API endpoints offline during updates.
* **Audit Logs:** Detailed history tracking of machine errors ```MachineErrorLogsController``` to help identify recurring hardware malfunctions.

### 📱 Modern UX/UI
* **Dark Mode:** Fully supported, system-wide dark theme that respects user preferences.
* **Responsive Design:** Optimized layout for Desktop, Tablet, and Mobile usage.
* **Smooth Animations:** Powered by **Framer Motion** for a premium, app-like feel.

---

## 🛠 Tech Stack

| Area | Technology |
| :--- | :--- |
| **Backend** | .NET 8, ASP.NET Core Web API, Entity Framework Core |
| **Frontend** | React 18, Vite, Tailwind CSS v4, Framer Motion |
| **Database** | SQL Server, Azure Blob Storage (Data Protection Keys) |
| **Real-time** | SignalR (WebSockets) |
| **DevOps** | Docker, Azure Container Registry (ACR), Azure Container Apps |
| **Mapping** | Leaflet.js, OpenStreetMap |
| **Testing** | xUnit, InMemory Database |

---

## 📸 Screenshots

![image alt](https://github.com/NeKsiKZ/VendingManager/blob/main/readmepng/dashboard.png?raw=true)

![image alt](https://github.com/NeKsiKZ/VendingManager/blob/main/readmepng/swagger.png?raw=true)

![image alt](https://github.com/NeKsiKZ/VendingManager/blob/main/readmepng/frontend.png?raw=true)


## ⚡ Getting Started

### Prerequisites
* [Docker Desktop](https://www.docker.com/products/docker-desktop)
* [.NET 8 SDK](https://dotnet.microsoft.com/download)
* [Node.js](https://nodejs.org/) (v18+)

### 1. Clone the Repository
```bash
git clone https://github.com/NeKsiKZ/VendingManager.git
cd VendingManager
```
### 2. Configuring Environment Variables (API Keys)
Before launching, make sure you have configured your API keys.

**Backend** ```VendingManager/appsettings.json```:
```bash
"ApiKey": "Your_Secret_API_key"
```

**Frontend** ```VendingManager.Client/.env```:
```bash
VITE_API_KEY=Your_Secret_API_key
VITE_API_URL=http://localhost:5000/api/machine
```

### 3. Run with Docker Compose (Recommended)
```bash
docker-compose up --build
```

### 4. Run Frontend
Open a new terminal tab for the client application.
```bash
cd VendingManager.Client
npm install
npm run dev
```
The application will be available at ```http://localhost:5173```.

---

## ☁️ Deployment
The project is configured for cloud deployment using Containerization.

**Backend (Azure)**

The backend is containerized and pushed to Azure Container Registry (ACR), then deployed to Azure Container Apps for scalability and ease of management.

* **Ingress:** Configured on port 8080.
* **Scaling:** Serverless consumption plan (scales to 0 when idle).

**Frontend (Vercel)**

The React client is deployed on **Vercel**, utilizing modern CI/CD pipelines connected directly to the GitHub repository.

---

## 📄 API Documentation
The API is fully documented using **Swagger**. Once the backend is running locally, you can explore the endpoints at:

```http://localhost:8080/swagger```

Or on the live Azure environment (if deployed): ```https://vendingmanager-app-bz123.purpleflower-915564b1.polandcentral.azurecontainerapps.io/swagger```

---

## 👤 Author

Bartosz Zimnoch

GitHub: https://github.com/NeKsiKZ

LinkedIn: [https://www.linkedin.com/in/bartosz-zimnoch-342113255/](https://www.linkedin.com/in/zimnoch/)

