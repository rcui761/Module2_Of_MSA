using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using System.Collections.Generic;

public class AzureManager
{

    private static AzureManager instance;
    private MobileServiceClient client;
    private IMobileServiceTable<kiwifruitmodel> kiwifruitTable;

    private AzureManager()
    {
        this.client = new MobileServiceClient("http://kiwifruit1995.azurewebsites.net");
        this.kiwifruitTable = this.client.GetTable<kiwifruitmodel>();
    }

    public MobileServiceClient AzureClient
    {
        get { return client; }
    }

    public static AzureManager AzureManagerInstance
    {
        get
        {
            if (instance == null)
            {
                instance = new AzureManager();
            }

            return instance;
        }
    }
    public async Task<List<kiwifruitmodel>> GetkiwifruitInformation()
    {
        System.Diagnostics.Debug.Write("kiwi fruit table"+kiwifruitTable);
        System.Diagnostics.Debug.Write("kiwi fruit table" + this.kiwifruitTable.ToListAsync());
        return await this.kiwifruitTable.ToListAsync();
        
    }
    public async Task PostkiwifruitInformation(kiwifruitmodel kiwifruitModel)
    {
        await this.kiwifruitTable.InsertAsync(kiwifruitModel);
    }
}