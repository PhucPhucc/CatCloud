import { Routes, Route, BrowserRouter } from "react-router";
import { Toaster } from "sonner";

import Folder from "./pages/Protected/Folder";
import Login from "./pages/Auth/Login";
import AppLayout from "./components/layouts/AppLayout";
import PrivatedLayout from "./components/layouts/PrivatedLayout";

function App() {
  return (
    <>
      <Toaster />
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<Login />} />

          <Route element={<PrivatedLayout />}>
            <Route element={<AppLayout />}>
              <Route path="/:id?" element={<Folder />} />
            </Route>
          </Route>

          <Route path="*" element={<div>Not Found</div>} />
        </Routes>
      </BrowserRouter>
    </>
  );
}

export default App;
