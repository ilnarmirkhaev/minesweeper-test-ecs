# Minesweeper Test Task

Клон игры "Сапёр", реализованный на Unity с использованием архитектурных паттернов ECS (LeoECSLite) и Dependency Injection (VContainer).

## Как играть

- **Левый клик мыши** - открыть клетку
- **Правый клик мыши** - поставить/убрать флажок
- **Клавиша R** - начать игру заново

## Настройка игры

Параметры игры настраиваются через `MineFieldConfig`:
- `Rows` - количество строк поля
- `Columns` - количество колонок поля
- `MinesCount` - количество мин

## Архитектура проекта

### Основные технологии
- **Unity** и **C#**
- **LeoECSLite** - фреймворк для ECS
- **VContainer** - контейнер для Dependency Injection

### Структура проекта

#### 1. Конфигурация (`Configs/`)
- `MineFieldConfig.cs` - ScriptableObject для настройки размера поля и количества мин

#### 2. Инфраструктура (`Infrastructure/`)
- `AppRoot.cs` - корневой DI контейнер, регистрирует все системы, сервисы и пулы
- `WorldRoot.cs` - управляет жизненным циклом ECS мира
- `BuilderExtensions.cs` - расширения для регистрации систем в DI

#### 3. Ядро (`Core/`)

**Компоненты (`Components/`)**
- `CellComponent` - позиция клетки
- `MineComponent` - наличие мины в клетке  
- `NeighborMinesCount` - количество мин вокруг клетки
- `Opened` - клетка открыта
- `Flagged` - клетка помечена флажком
- `Exploded` - клетка взорвана
- `Dirty` - клетка требует обновления отображения
- События и запросы: `ClickCellRequest`, `OpenCellCommand`, `FirstCellClickedEvent` и др.

**Системы (`Systems/`)**
- **Системы ввода:** `RestartInputSystem`, `CellClickSystem`
- **Системы инициализации:** `FieldCreationSystem`, `MineDistributionSystem`, `NeighborMinesCountSystem`
- **Игровые системы:** `CellOpenSystem`, `CellFlagSystem`, `WinCheckSystem`, `CellViewDrawSystem`
- **Системы сброса:** `GameResetSystem`, `GameStartCleanupSystem`

**Сервисы (`Services/`)**
- `GameSessionState` - состояние игровой сессии
- `ICellLookup` - поиск клеток по позиции
- `ICellInputService` - обработка ввода
- `ICellViewRegistry` - реестр отображений клеток

#### 4. Пользовательский интерфейс (`UI/`)
- `CellView.cs` - отображение отдельной клетки
- `FieldView.cs` - отображение всего поля
- `GameOverView.cs` - экран окончания игры
- `CellViewRegistry.cs` - управление отображениями клеток

#### 5. Инструменты (`Tools/`)
- `Constants.cs` - константы игры (смещения соседей)

## Особенности реализации
- **Безопасный первый клик** - система `MineDistributionSystem` гарантирует отсутствие мины в первой открытой клетке и её соседях
- **Flood Fill алгоритм** - система `CellOpenSystem` реализует автоматическое открытие пустых областей
- **ECS архитектура** - вся игровая логика реализована через системы, обрабатывающие компоненты
- **DI контейнер** - все зависимости внедряются через VContainer

## Поток выполнения игры

1. **Инициализация** - `FieldCreationSystem` создаёт поле клеток
2. **Первый клик** - `CellClickSystem` обрабатывает, `MineDistributionSystem` расставляет мины
3. **Расчёт соседей** - `NeighborMinesCountSystem` вычисляет количество мин вокруг каждой клетки
4. **Игровой цикл** - системы обрабатывают ввод, открывают клетки, проверяют победу/поражение
5. **Отображение** - `CellViewDrawSystem` обновляет визуальное представление на основе компонентов
