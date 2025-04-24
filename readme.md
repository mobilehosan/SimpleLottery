# Simple Lottery

## Notes About the Implementation

### ✅ Implemented
1. Simplified DDD approach without events, but with a rich domain model — all business logic is encapsulated within the Domain project.
2. Applied Clean Architecture principles.
3. Added one behavioral test for a general case (based on the example with standard preferences).
4. For test used FluentAssertions, Moq and Xunit.
5. Tiers and lottery options are fully configurable via `appsettings.json`.

### 🚧 Possible Improvements
1. More unit tests for various scenarios (especially for multiple drawings) could be added.
2. Currently, no logging, persistence layer, or web UI is implemented.
3. The current UI mirrors the example from the PDF — there's potential to make it more interactive or user-friendly, as all necessary data is available.
4. Enhanced validation rules and types would be valuable.
5. Presentation layer could be refactored into separate classes for clarity and scalability.

## 🛠️ How to Run

Navigate to the `Presentation` project and run:

```bash
cd SimplifiedLotteryGame/SimpleLottery.Presentation
dotnet run

