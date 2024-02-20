import React from "react";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { CreateReport } from "../CreateReport";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: false,
    },
  },
});

it("renders the screen", async () => {
  render(
    <QueryClientProvider client={queryClient}>
      <CreateReport></CreateReport>
    </QueryClientProvider>
  );

  expect(screen.getByText(/Print Report/i)).toBeInTheDocument();
});
