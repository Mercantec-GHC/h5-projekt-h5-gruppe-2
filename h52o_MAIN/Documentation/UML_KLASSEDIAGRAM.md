# Teknisk Arkitektur

Dette dokument beskriver systemets opbygning og datamodel. Arkitekturen er bygget op omkring en lagdelt struktur med klare ansvarsområder.

## Klassediagram (UML)

Herunder ses projektets klassediagram, som visualiserer forholdet mellem vores POCO-modeller, database-context og repositories.

```mermaid
classDiagram
    direction TB

    namespace Models {
        class User {
            +int Id
            +string Username
            +string PasswordHash
            +bool IsAdmin
            +Device? Device
        }
        class Device {
            +int Id
            +string DeviceKey
            +bool IsActive
            +int UserId
            +List~MeasurementSet~ Sets
        }
        class MeasurementSet {
            +int Id
            +string? Location
            +DateTime StartTime
            +DateTime? EndTime
            +int DeviceId
            +List~Measurement~ Measurements
        }
        class Measurement {
            +int Id
            +int Value
            +DateTime Timestamp
            +int MeasurementSetId
        }
    }

    %% Relationer placeret her for at tvinge vertikal stakning
    User "1" -- "0..1" Device : owns
    Device "1" -- "*" MeasurementSet : hosts
    MeasurementSet "1" -- "*" Measurement : collects

    namespace DTOs {
        class UserDTO {
            +int Id
            +string Username
            +bool IsAdmin
            +int? DeviceId
            +bool IsDeviceActive
            +List~int~ MeasurementSetIds
        }
        class DeviceDTO {
            +int Id
            +string DeviceKey
            +bool IsActive
            +int UserId
            +List~int~ MeasurementSetIds
        }
        class MeasurementSetDTO {
            +int Id
            +string? Location
            +DateTime StartTime
            +DateTime? EndTime
            +int DeviceId
            +List~int~ MeasurementIds
        }
        class UserMeasurementSetDTO {
            +int Id
            +string? Location
            +DateTime StartTime
            +DateTime? EndTime
        }
        class MeasurementDTO {
            +int Id
            +int Value
            +DateTime Timestamp
            +int MeasurementSetId
            +int DeviceId
        }
    }

    %% Mapping relationer
    User ..> UserDTO : mapped to
    Device ..> DeviceDTO : mapped to
    MeasurementSet ..> UserMeasurementSetDTO : list
    MeasurementSet ..> MeasurementSetDTO : detail