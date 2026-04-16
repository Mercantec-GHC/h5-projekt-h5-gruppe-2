# Teknisk Arkitektur

Dette dokument beskriver systemets opbygning og datamodel. Arkitekturen er bygget op omkring en lagdelt struktur med klare ansvarsområder.

## Domænemodel

Herunder ses projektets domænemodel

```mermaid
classDiagram
    direction TB

    class User {
        Username
        Password
        AdminStatus
    }

    class Device {
        DeviceKey
        ActiveStatus
    }

    class MeasurementSet {
        Location
        StartTime
        EndTime
    }

    class Measurement {
        Value
        Timestamp
    }

    %% Relationer med forretningslogik
    User "1" -- "0..1" Device : ejer
    Device "1" -- "*" MeasurementSet : registrerer
    MeasurementSet "1" -- "*" Measurement : indeholder

    %% Beskrivende noter til modellen
    note for User "En bruger kan eje én måleenhed"
    note for MeasurementSet "En session af målinger i en specifik lokation"
    note for Measurement "Rå data opsamlet fra sensorer"