# Fuel Downloader - Angular UI

A web interface for viewing and managing diesel fuel price data from the U.S. Energy Information Administration (EIA), built with Angular 20.

**This is the frontend layer** that completes the full-stack implementation, connecting to the C# or Go backend APIs to provide a user-friendly interface for fuel price data.

## Why This Project Exists

This UI solves a **real business problem**: I create freight invoices for my corporation that require current diesel fuel prices. Instead of manually looking up prices or running CLI commands, this web interface provides:
- Quick access to current fuel prices
- Historical price trends and visualization
- Export functionality for invoicing workflows
- Regional price comparisons

## Architecture

Built using **Vertical Slice Architecture** with Angular best practices:

```
src/
├── app/
│   ├── features/           # Feature modules (vertical slices)
│   │   ├── fuel-rates/     # Fuel rate management
│   │   ├── dashboard/      # Overview and charts
│   │   └── export/         # CSV export functionality
│   ├── core/               # Singleton services, guards
│   │   ├── services/       # API clients, state management
│   │   └── interceptors/   # HTTP interceptors
│   ├── shared/             # Shared components, pipes, directives
│   └── models/             # TypeScript interfaces/types
└── environments/           # Environment configurations
```

The structure mirrors the vertical slice approach used in the C# and Go backends, maintaining architectural consistency across the full stack.

## Current Implementation

The Angular UI provides:
- ✅ Component architecture with standalone components
- ✅ TypeScript strict mode for type safety
- ✅ Reactive forms for data entry
- ✅ HTTP client integration for backend APIs
- ✅ Responsive design for desktop and mobile
- 🔄 Real-time price updates (in progress)
- 🔄 Historical price charts (planned)
- 🔄 Multi-region comparison (planned)

## Features

- **Dashboard View** - Current fuel prices and trends
- **Price History** - Historical data visualization
- **Export Functionality** - Generate CSV files for invoicing
- **Region Selector** - View prices by geographic area
- **Responsive Design** - Works on desktop and mobile devices

## Prerequisites

- Node.js 20.x+
- Angular CLI 20.3.3+
- Backend API (C# or Go implementation) running

## Quick Start

### 1. Install Dependencies

```bash
cd angular
npm install
```

### 2. Configure API Endpoint

Update `src/environments/environment.development.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api'  // C# API
  // or
  // apiUrl: 'http://localhost:8080/api'  // Go API
};
```

### 3. Start Development Server

```bash
ng serve
```

Navigate to `http://localhost:4200/` - the app will automatically reload when you modify source files.

### 4. Build for Production

```bash
ng build
```

Build artifacts will be stored in the `dist/` directory, optimized for production deployment.

## Technology Stack

- **Angular 20** - Modern web framework
- **TypeScript** - Type-safe JavaScript
- **RxJS** - Reactive programming
- **Angular Material** (optional) - UI component library
- **Chart.js** (planned) - Data visualization
- **Standalone Components** - Modern Angular architecture

## Project Structure

```
fuel-downloader-ui/
├── src/
│   ├── app/
│   │   ├── features/
│   │   │   ├── fuel-rates/
│   │   │   │   ├── fuel-rates.component.ts
│   │   │   │   ├── fuel-rates.component.html
│   │   │   │   ├── fuel-rates.component.css
│   │   │   │   └── fuel-rates.service.ts
│   │   │   ├── dashboard/
│   │   │   │   └── dashboard.component.ts
│   │   │   └── export/
│   │   │       └── export.component.ts
│   │   ├── core/
│   │   │   └── services/
│   │   │       ├── api.service.ts
│   │   │       └── fuel-rate.service.ts
│   │   ├── shared/
│   │   │   ├── components/
│   │   │   └── pipes/
│   │   ├── models/
│   │   │   └── fuel-rate.model.ts
│   │   └── app.component.ts
│   ├── environments/
│   │   ├── environment.ts
│   │   └── environment.development.ts
│   └── index.html
├── angular.json
├── package.json
└── tsconfig.json
```

## API Integration

The Angular UI integrates with either backend implementation:

### C# Backend API
```typescript
// Default endpoint: http://localhost:5000/api
GET /api/fuel-rates/latest       // Get latest rate
GET /api/fuel-rates?area=NUS     // Get rates by area
GET /api/fuel-rates/history      // Get historical data
POST /api/fuel-rates/export      // Generate CSV
```

### Go Backend API
```typescript
// Default endpoint: http://localhost:8080/api
GET /api/fuel-rates/latest       // Get latest rate
GET /api/fuel-rates?area=NUS     // Get rates by area
GET /api/fuel-rates/history      // Get historical data
POST /api/fuel-rates/export      // Generate CSV
```

Both backends expose identical REST APIs, allowing seamless frontend integration with either implementation.

## Key Design Patterns

- **Vertical Slice Architecture** - Feature-focused organization
- **Reactive Programming** - RxJS observables for async operations
- **Service Layer Pattern** - Business logic in services
- **Smart/Dumb Components** - Container and presentational components
- **Dependency Injection** - Angular's built-in DI system
- **Type Safety** - TypeScript interfaces for data models

## Sample Data Model

```typescript
export interface FuelRate {
  productCode: string;      // "EPD2D"
  productName: string;      // "No 2 Diesel"
  areaCode: string;         // "NUS"
  areaName: string;         // "U.S."
  period: string;           // "2025-08"
  value: number;            // 3.744
  unit: string;             // "$/GAL"
  generatedUtc: Date;       // Timestamp
}
```

## Design Philosophy

This Angular implementation demonstrates:
1. **Full-Stack Capability** - Frontend completes the C#/Go backend projects
2. **Vertical Slice Consistency** - Same architectural approach across all layers
3. **Modern Angular Patterns** - Standalone components, strict TypeScript
4. **API-Agnostic Design** - Works with either C# or Go backend
5. **Production-Ready UI** - Not a tutorial, but a real interface

## Running Tests

### Unit Tests
```bash
ng test
```

### End-to-End Tests
```bash
ng e2e
```

## Deployment

### Docker (Planned)
```dockerfile
FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist/fuel-downloader-ui /usr/share/nginx/html
EXPOSE 80
```

### AWS S3 + CloudFront (Planned)
Static site hosting with CDN distribution for optimal performance.

## Future Enhancements

The Angular UI will continue to evolve with:
- Real-time price updates via WebSockets
- Interactive charts with Chart.js or D3.js
- Multi-region price comparison view
- Price alert notifications
- User preferences and saved views
- Comprehensive E2E test coverage
- Progressive Web App (PWA) capabilities

## Integration with Backend Projects

| Backend | API Endpoint | Status |
|---------|--------------|--------|
| **C# API** | http://localhost:5000/api | ✅ Compatible |
| **Go API** | http://localhost:8080/api | ✅ Compatible |

Both backends implement the same REST contract, allowing the Angular UI to work with either implementation seamlessly.

## Related Projects

- **C# Implementation** - `/csharp` - ASP.NET Core API
- **Go Implementation** - `/go` - Chi router API
- **Cloud Pipeline** - CI/CD deployment (dtorres-cloud-pipeline)

---

*This Angular UI completes the full-stack implementation, demonstrating frontend development skills alongside the C# and Go backend architectures while maintaining vertical slice consistency across all layers.*
