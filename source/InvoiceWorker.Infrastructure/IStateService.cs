using System.Threading.Tasks;

namespace InvoiceWorker.Infrastructure
{
    /// <summary>
    /// Interface to manage state of the application.
    /// </summary>
    public interface IStateService
    {
        /// <summary>
        /// Gets the current state.
        /// </summary>
        Task<State> GetState();
        /// <summary>
        /// Saves the state.
        /// </summary>
        Task SaveState(State state);
    }
}
