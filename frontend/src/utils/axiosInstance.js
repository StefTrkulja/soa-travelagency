import axios from 'axios';
import { defaultConfig } from '@/config/config';
import router from '@/router';
import { store } from '@/utils/store';

const axiosInstance = axios.create({
	baseURL: defaultConfig.baseURL,
	withCredentials: true
});

axiosInstance.interceptors.request.use(
	config => {
		const token = localStorage.getItem('token');
		if (token) {
			config.headers.Authorization = `Bearer ${token}`;
		}
		return config;
	},
	error => {
		return Promise.reject(error);
	}
);

axiosInstance.interceptors.response.use(
	res => res,
	error => {
		if (error.response && (error.response.status === 401 || error.response.status === 403)) {
			store.clearUser();
			localStorage.removeItem('token');
			router.replace({ path: '/login' });
		}

		return Promise.reject(error);
	}
);

export default axiosInstance;