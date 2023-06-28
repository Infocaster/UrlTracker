import { Axios } from "axios";

export const axiosInstance = new Axios({
    transformResponse: [
        (data) => {
            return JSON.parse(data.substring(6));
        }
    ]
});