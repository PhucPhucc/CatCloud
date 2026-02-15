import { Routes, Route, BrowserRouter } from "react-router";
import { Toaster } from "sonner";

import Folder from "./pages/Protected/Folder";
import Login from "./pages/Auth/Login";

function App() {
  return (
    <>
      <Toaster />
      <BrowserRouter>
        <Routes>
          <Route index element={<Folder />} />
          <Route path="/login" element={<Login />} />
        </Routes>
      </BrowserRouter>
    </>
  );
}

export default App;
