"use client"
import { useState } from "react";
import { redirect } from "next/navigation";


export default function Home() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [username, setUsername] = useState("");
  const [isLogin, setIsLogin] = useState(true)

  const handleSignUp = async (e) => {
    const body = {
      email,
      username,
      password,
      role: "User",
    };

    const response = await fetch('api/Auth/Register', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(body),
    });

    if (!response.ok)
      console.error('SignUp failed');

  }

  const handleLogin = async (e) => {
    e.preventDefault();
    const body = {
      email,
      password,
    };

    const response = await fetch('api/Auth/Login', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(body),
    });

    if (response.ok) {
      redirect("/SolarWatch")
    } else {
      console.error('Login failed');
    }
  } 


  return (
    <main className="home">
      <div className="top"></div>
      <div className="bottom"></div>
      <form className="content" onSubmit={(e) => isLogin ? handleLogin(e) : handleSignUp(e)}>
        <h2 className="header">
          <span className={`${isLogin ? "active" : ""}`} onClick={() => setIsLogin(true)}>
            Sign in
          </span>
          <span> / </span>
          <span className={`${isLogin ? "" : "active"}`} onClick={() => setIsLogin(false)}>
            Sign up
          </span>
        </h2>
        <input
          type="email"
          placeholder="Email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
        />
        {!isLogin && (
          <input
            type="text"
            placeholder="Username"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
          />
        )}
        <input
          type="password"
          placeholder="Password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />
        <button type="submit">{isLogin ? "Login" : "Sing up"}</button>
      </form>
    </main>
  );
}
