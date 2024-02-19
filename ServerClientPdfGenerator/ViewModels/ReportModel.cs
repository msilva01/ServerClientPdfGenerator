namespace ServerClientPdfGenerator.ViewModels;

public class ReportModel
{
    public UserModel UserDto { get; set; }
    
    
}

public class ReportDataModel
{
    public string DateIssued { get; set; }
    public string ClientName { get; set; }
    public string ItemBought { get; set; }
    public string Amount { get; set; }
}