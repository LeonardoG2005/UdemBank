# UdemBank

Sistema web de gestión bancaria cooperativa que permite administrar grupos de ahorro, solicitar préstamos y realizar transacciones.

## Descripción

UdemBank es una aplicación web desarrollada con ASP.NET Core MVC que simula un sistema bancario cooperativo. Los usuarios pueden crear grupos de ahorro, solicitar préstamos y gestionar sus finanzas de forma colaborativa.

### Características principales

- Gestión de usuarios y cuentas bancarias
- Creación de grupos de ahorro (máximo 3 por usuario)
- Solicitud de préstamos con intereses calculados
- Registro de transacciones
- Sistema de recompensas por fidelización

## Tecnologías

- .NET 6.0
- ASP.NET Core MVC
- Entity Framework Core 7.0
- SQLite
- Bootstrap 5

## Requisitos

- .NET SDK 6.0 o superior

## Instalación

1. Clonar el repositorio
2. Navegar a la carpeta del proyecto
3. Restaurar dependencias:
   ```bash
   dotnet restore
   ```

## Ejecución

Ejecutar la aplicación desde la raíz del proyecto:

```bash
dotnet run --project UdemBank/UdemBank.csproj
```

La aplicación estará disponible en `https://localhost:5001` o `http://localhost:5000`

## Base de datos

La aplicación utiliza SQLite como motor de base de datos. El archivo `udem_bank.db` se crea automáticamente en la primera ejecución dentro de la carpeta `UdemBank/UdemBank/`.

No se requiere instalación ni configuración adicional de base de datos.

## Estructura

```
UdemBank/
├── Controllers/        # Lógica de controladores MVC
├── Models/            # Modelos de datos
├── Views/             # Vistas Razor
├── wwwroot/           # Recursos estáticos
├── Program.cs         # Punto de entrada
└── UdemBankContext.cs # Contexto de base de datos
```

## Reglas de negocio

- Un usuario puede pertenecer a máximo 3 grupos de ahorro
- Los préstamos tienen un plazo mínimo de 2 meses
- Los intereses se calculan según el plazo del préstamo
- Los usuarios premiados reciben 1% menos en intereses
- El usuario debe estar afiliado al grupo para solicitar préstamos

## Licencia

Este proyecto es de código abierto y está disponible bajo la licencia MIT.

