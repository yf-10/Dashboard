var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
const FetchDataAsync = (url) => __awaiter(this, void 0, void 0, function* () {
    const response = yield fetch(url, {
        mode: "cors"
    });
    console.log(response);
    if (!response.ok) {
        throw new Error('Server response error');
    }
    const responseJson = yield response.json();
    return responseJson;
});
const FetchDataWithHeaderAsync = (url, headers) => __awaiter(this, void 0, void 0, function* () {
    const response = yield fetch(url, {
        mode: "cors",
        headers: headers
    });
    console.log(response);
    if (!response.ok) {
        throw new Error('Server response error');
    }
    const responseJson = yield response.json();
    return responseJson;
});
const GetOrigin = () => {
    if (location.pathname.indexOf('dashboard') > 0) {
        return location.origin + '/dashboard';
    }
    else {
        return location.origin;
    }
};
//# sourceMappingURL=common.js.map