import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Field, FieldGroup, FieldLabel } from "@/components/ui/field";
import { Input } from "@/components/ui/input";
import api from "@/lib/api";
import { useNavigate } from "react-router-dom";
import { useAuthStore } from "@/store/authStore";
import { toast } from "sonner";

export function LoginForm({
  className,
  ...props
}: React.ComponentProps<"div">) {
  const login = useAuthStore((state) => state.login);
  const navigate = useNavigate();

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    // setIsLoading(true);
    const formData = new FormData(e.target as HTMLFormElement);
    const username = formData.get("username") as string;
    const password = formData.get("password") as string;
    try {
      const res = await api.post("/auth/login", { username, password });
      const token = res.data.accessToken; // Sửa lại key này theo đúng API BE trả về
      console.log(res.data);
      // Lưu vào Store
      login({ id: "1", username }, token);

      toast.success("Đăng nhập thành công!");
      navigate("/");
    } catch (error: any) {
      toast.error(error.response?.data || "Đăng nhập thất bại");
    } finally {
      // setIsLoading(false);
    }
  };
  return (
    <div className={cn("flex flex-col gap-6", className)} {...props}>
      <Card>
        <CardHeader className="text-2xl text-center">
          <CardTitle>Đăng nhập</CardTitle>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleLogin}>
            <FieldGroup>
              <Field>
                <FieldLabel htmlFor="username">Tài Khoản</FieldLabel>
                <Input
                  id="username"
                  name="username"
                  type="text"
                  placeholder="Tên tài khoản"
                  required
                />
              </Field>
              <Field>
                <FieldLabel htmlFor="password">Mật Khẩu</FieldLabel>
                <Input
                  id="password"
                  type="password"
                  name="password"
                  placeholder="Mật khẩu"
                  required
                />
              </Field>
              <Field>
                <Button type="submit">Đăng Nhập</Button>
                <Button variant="outline">Đăng nhập tài khoản dùng thử</Button>
              </Field>
            </FieldGroup>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}
