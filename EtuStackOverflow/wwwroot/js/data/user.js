import { getCookie, deleteCookie } from '../cookieManager.js';

export const getUserProfileDetail = async (app) => {
    const tokenCookieValue = getCookie("accessToken")

    if (tokenCookieValue === "") {
        app.isLogin = false;
        deleteCookie("accessToken");
        window.location.pathname = "/";
        return;
    } else {
        app.token = tokenCookieValue;
    }

    const data = localStorage.getItem('userProfileDetail');

    if (data) {
        app.userProfileDetail = JSON.parse(data);
        fillEditProfile(app);
        return;
    }

    try {
        const response = await axios.get(`/api/users/profile-detail`, { headers: { 'Authorization': 'Bearer ' + app.token } });
        app.userProfileDetail = response.data.data;

        //save localstorge like cache data
        localStorage.setItem('userProfileDetail', JSON.stringify(app.userProfileDetail));

        fillEditProfile(app);

        console.log(app.userProfileDetailEdit)
    } catch (error) {
        const statusCode = error.response.status;
        if (statusCode === 401) {
            app.isLogin = false;
            deleteCookie("accessToken");
            window.location.pathname = "/";
        }
        throw error;
    }
}

export const updateUserProfileDetail = async (app) => {
    const tokenCookieValue = getCookie("accessToken")

    if (tokenCookieValue === "") {
        app.isLogin = false;
        deleteCookie("accessToken");
        window.location.pathname = "/";
        return;
    } else {
        app.token = tokenCookieValue;
    }

    try {
        await axios.post(`/api/users/update-profile-detail`,
            app.userProfileDetailEdit,
            { headers: { 'Authorization': 'Bearer ' + app.token } });
        window.location.pathname = "/profile";
    } catch (error) {
        const statusCode = error.response.status;
        if (statusCode === 401) {
            app.isLogin = false;
            deleteCookie("accessToken");
            window.location.pathname = "/";
            return;
        }
        app.errorMessage = error.response.data.messages[0];
        app.isError = true;
        throw error;
    }
}

function fillEditProfile(app) {
    app.userProfileDetailEdit.name = app.userProfileDetail.name;
    app.userProfileDetailEdit.surName = app.userProfileDetail.surName;
    app.userProfileDetailEdit.dateOfBirth = app.userProfileDetail.dateOfBirth;
    app.userProfileDetailEdit.profilePhoto = app.userProfileDetail.profilePhoto;
    app.userProfileDetailEdit.userName = app.userProfileDetail.userName;
}