import axios from "axios";

// Instancia o Axios apontando estritamente para a variável de ambiente
const http = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
});

// Interceptor para injetar o Token automaticamente
http.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  if (token && config.headers) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export default http;
