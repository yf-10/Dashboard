// Class Definition
class Api {
    // HTML Element
    private btnGenerateApiKey: HTMLElement;
    private txtUserID: HTMLElement;
    private txtApiKey: HTMLElement;
    private btnTest: HTMLElement;

    // Constructor
    public constructor() {
        // HTML Element
        this.btnGenerateApiKey = document.getElementById('btnGenerateApiKey') as HTMLElement | null;
        this.txtUserID = document.getElementById('txtUserID') as HTMLElement | null;
        this.txtApiKey = document.getElementById('txtApiKey') as HTMLElement | null;
        this.btnTest = document.getElementById('btnTest') as HTMLElement | null;

        // Set User ID
        this.txtUserID.innerText = "admin";

        // Add Button Click Event Listener
        if (this.btnGenerateApiKey) {
            this.btnGenerateApiKey.addEventListener('click', async (event) => {
                await this.GenerateApiKeyAsync();
            })
        }

        // Add Button Click Event Listener
        if (this.btnTest) {
            this.btnTest.addEventListener('click', async (event) => {
                await this.TestAsync();
            })
        }
    }

    // Generate API Key
    private GenerateApiKeyAsync = async (): Promise<void> => {
        const userId: string = this.txtUserID.innerText;
        try {
            const url = GetOrigin() + "/api/key/" + userId;
            console.log(url);
            const responseJson: ResponseJson = await FetchDataAsync(url);
            console.log(responseJson);
            if (responseJson.status == 0) {
                const key: string = responseJson.data;
                this.txtApiKey.innerText = key;
                ShowInfoToast("Success to fetch API key");
            } else {
                throw new Error('Failed to fetch API Key');
            }
        } catch (error) {
            console.error(error);
            ShowErrorToast(error);
            this.txtApiKey.innerText = "***";
        }
    }

    // Fetch Header Test
    private TestAsync = async (): Promise<void> => {
        const userId: string = this.txtUserID.innerText;
        try {
            const url = GetOrigin() + "/api/batchlogs";
            console.log(url);
            const headers = {
                'x-user-id': 'admin',
                'x-api-key': '6f625426-7252-4eb4-a5f5-c2b65aa85760'
            }
            const responseJson: ResponseJson = await FetchDataWithHeaderAsync(url, headers);
            console.log(responseJson);
            if (responseJson.status == 0) {
                const key: string = responseJson.data;
                this.txtApiKey.innerText = key;
                ShowInfoToast("Success to fetch API key");
            } else {
                throw new Error('Failed to fetch API Key');
            }
        } catch (error) {
            console.error(error);
            ShowErrorToast(error);
            this.txtApiKey.innerText = "***";
        }
    }

}

// Create Instance
const apiInstance = new Api();
