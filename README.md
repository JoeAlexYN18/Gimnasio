# GIMNASIO

Este repositorio contiene:

* Entidades con **Entity Framework Core**
* Servicios
* Controladores
* Documentación técnica completa

---

## Instrucciones de instalación

1. Clonar el repositorio:

   ```bash
   git clone https://github.com/JoeAlexYN18/Gimnasio.git
   ```

2. Configurar la cadena de conexión en `appsettings.json`:

   * `Host`
   * `Database`
   * `Username`
   * `Password`

3. Configurar la clave JWT en:

   ```
   JwtSettings:SecretKey
   ```

   > Debe tener un mínimo de 32 caracteres.

4. Ejecutar las migraciones:

   ```bash
   dotnet ef database update
   ```

5. Iniciar la aplicación:

   ```bash
   dotnet run
   ```

6. Acceder a Swagger UI:

   ```
   https://localhost:{PORT}/
   ```

---

## Flujo completo de ejecución (ALTERNATIVO)

Después de configurar la cadena de conexión y la clave JWT, 
puedes ejecutar el proyecto limpiamente desde cero usando los siguientes
comandos de manera secuencial:

```bash
dotnet restore
dotnet ef migrations add InitialCreate --output-dir Migrations
dotnet ef database update
dotnet run
```

---

## Notas

* Las dependencias se restauran automáticamente.
* La base de datos se crea junto con sus relaciones.
* No se requiere configuración adicional después de estos pasos.
---
