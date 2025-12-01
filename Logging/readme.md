## Logging

Se provee un sistema de logging sencillo, extensible y thread-safe en `DynamicUI.Logging`.

### Targets incluidos
- ColoredConsoleLogTarget
- ConsoleLogTarget
- FileLogTarget
- JsonFileLogTarget
- MemoryLogTarget
- CompositeLogTarget
- NetworkLogTarget (ejemplo)

### Uso básico
```csharp
var logger = new UILogger();
logger.SetMinLevel(LogLevel.Debug);
logger.AddTarget(new ColoredConsoleLogTarget());
logger.AddTarget(new JsonFileLogTarget("logs/app.json"));
logger.LogInfo("Iniciado");
