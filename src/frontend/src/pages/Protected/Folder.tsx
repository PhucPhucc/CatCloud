import api from "@/lib/api";
import { useEffect } from "react";

const Folder = () => {
  useEffect(() => {
    api.get("/folders").then((res) => {
      console.log(res.data);
    });
  }, []);

  return <div>Folder</div>;
};

export default Folder;
