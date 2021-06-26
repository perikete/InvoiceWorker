using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using InvoiceWorker.Infrastructure;

namespace InvoiceWorker
{
    /// <summary>
    /// A local json file implementation for the storing runner's state.
    /// </summary>
    public class JsonStateService : IStateService
    {
        private const string StateFilename = "current_state.json";

        /// <inheritdoc />
        public async Task<State> GetState()
        {
            if (!File.Exists(StateFilename))
            {
                var newState = new State { LastEventId = 0 };
                var serializedState = JsonSerializer.Serialize(newState);
                await File.WriteAllTextAsync(StateFilename, serializedState);

                return newState;
            }

            var stateJson = await File.ReadAllTextAsync(StateFilename);
            return JsonSerializer.Deserialize<State>(stateJson);
        }

        /// <inheritdoc />
        public async Task SaveState(State state)
        {
            var serializedState = JsonSerializer.Serialize(state);
            await File.WriteAllTextAsync(StateFilename, serializedState);
        }
    }
}
