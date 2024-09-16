using Dashboard.Models.Data;

namespace Dashboard.Models.ViewModel; 
public class BatchViewModel(List<BatchlogMain> batchlogs) {
    public List<BatchlogMain> Batchlogs { get; private set; } = batchlogs;
}
