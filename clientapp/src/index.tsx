import React from "react";
import ReactDOM from "react-dom/client";
import "./index.css";
import App from "./App";
import {
  QueryCache,
  QueryClient,
  QueryClientProvider,
} from "@tanstack/react-query";
import { ToastContainer, toast } from "react-toastify";
import { SignalR } from "./Services/SignalRContext";
import "bootstrap/dist/css/bootstrap.min.css";

const root = ReactDOM.createRoot(
  document.getElementById("root") as HTMLElement
);
const queryClient = new QueryClient({
  queryCache: new QueryCache({
    onError: (error: any) => {
      toast.error(error?.response?.data?.message || error);
    },
  }),
  defaultOptions: {
    queries: {
      placeholderData: (prev: any) => prev,
      refetchOnWindowFocus: process.env.REACT_APP_ENVIRONMENT !== "DEV",
      staleTime: Infinity,
    },
  },
});

root.render(
  <React.StrictMode>
    <ToastContainer></ToastContainer>
    <QueryClientProvider client={queryClient}>
      <SignalR>
        <App />
      </SignalR>
    </QueryClientProvider>
  </React.StrictMode>
);
