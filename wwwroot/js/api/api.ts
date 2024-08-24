// From "toast.js"
declare function ShowInfoToast(message: string): void;
declare function ShowErrorToast(message: string): void;

// Class Definition
class Api {
    // HTML Element
    private btnGenerateApiKey: HTMLElement;
    private txtUserID: HTMLElement;
    private txtApiKey: HTMLElement;

    // Constructor
    public constructor() {
        // HTML Element
        this.btnGenerateApiKey = document.getElementById('btnGenerateApiKey') as HTMLElement | null;
        this.txtUserID = document.getElementById('txtUserID') as HTMLElement | null;
        this.txtApiKey = document.getElementById('txtApiKey') as HTMLElement | null;

        // Set User ID
        this.txtUserID.innerText = "admin";

        // Add Button Click Event Listener
        if (this.btnGenerateApiKey) {
            this.btnGenerateApiKey.addEventListener('click', (event) => {
                this.GenerateApiKey();
            })
        }
    }

    // Generate API Key
    private GenerateApiKey = async (): Promise<void> => {
        const userId: string = this.txtUserID.innerText;
        try {
            const url = GetOrigin() + "/api/key/" + userId;
            console.log(url);
            const responseJson: ResponseJson = await FetchData(url);
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
