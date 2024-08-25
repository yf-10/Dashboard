var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
// Class Definition
class Api {
    // Constructor
    constructor() {
        // Generate API Key
        this.GenerateApiKeyAsync = () => __awaiter(this, void 0, void 0, function* () {
            const userId = this.txtUserID.innerText;
            try {
                const url = GetOrigin() + "/api/key/" + userId;
                console.log(url);
                const responseJson = yield FetchDataAsync(url);
                console.log(responseJson);
                if (responseJson.status == 0) {
                    const key = responseJson.data;
                    this.txtApiKey.innerText = key;
                    ShowInfoToast("Success to fetch API key");
                }
                else {
                    throw new Error('Failed to fetch API Key');
                }
            }
            catch (error) {
                console.error(error);
                ShowErrorToast(error);
                this.txtApiKey.innerText = "***";
            }
        });
        // Fetch Header Test
        this.TestAsync = () => __awaiter(this, void 0, void 0, function* () {
            const userId = this.txtUserID.innerText;
            try {
                const url = GetOrigin() + "/api/batchlogs";
                console.log(url);
                const headers = {
                    'x-user-id': 'admin',
                    'x-api-key': '6f625426-7252-4eb4-a5f5-c2b65aa85760'
                };
                const responseJson = yield FetchDataWithHeaderAsync(url, headers);
                console.log(responseJson);
                if (responseJson.status == 0) {
                    const key = responseJson.data;
                    this.txtApiKey.innerText = key;
                    ShowInfoToast("Success to fetch API key");
                }
                else {
                    throw new Error('Failed to fetch API Key');
                }
            }
            catch (error) {
                console.error(error);
                ShowErrorToast(error);
                this.txtApiKey.innerText = "***";
            }
        });
        // HTML Element
        this.btnGenerateApiKey = document.getElementById('btnGenerateApiKey');
        this.txtUserID = document.getElementById('txtUserID');
        this.txtApiKey = document.getElementById('txtApiKey');
        this.btnTest = document.getElementById('btnTest');
        // Set User ID
        this.txtUserID.innerText = "admin";
        // Add Button Click Event Listener
        if (this.btnGenerateApiKey) {
            this.btnGenerateApiKey.addEventListener('click', (event) => __awaiter(this, void 0, void 0, function* () {
                yield this.GenerateApiKeyAsync();
            }));
        }
        // Add Button Click Event Listener
        if (this.btnTest) {
            this.btnTest.addEventListener('click', (event) => __awaiter(this, void 0, void 0, function* () {
                yield this.TestAsync();
            }));
        }
    }
}
// Create Instance
const apiInstance = new Api();
//# sourceMappingURL=api.js.map