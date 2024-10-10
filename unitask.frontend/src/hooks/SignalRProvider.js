import React, { useEffect, useState, createContext, useContext } from "react";
import { HubConnectionBuilder } from "@microsoft/signalr";
import useAuth from "../hooks/useAuth";
import { API_BASE_URL } from '../config';

const SignalRContext = createContext();

export const useSignalR = () => {
  return useContext(SignalRContext);
};

export const SignalRProvider = ({ children }) => {
  const [connection, setConnection] = useState(null);
  const [userStatus, setUserStatus] = useState({});
  const { id } = useAuth();

  useEffect(() => {
    const connect = async () => {
      const conn = new HubConnectionBuilder()
        .withUrl(`${API_BASE_URL}userStatusHub`)
        .build();

      conn.on("ReceiveUserStatus", (userId, isOnline) => {
        setUserStatus((prevStatus) => ({ ...prevStatus, [userId]: isOnline }));
      });

      await conn.start();
      setConnection(conn);

      const userId = id;
      conn.invoke("UpdateUserStatus", userId, true);

      window.addEventListener("beforeunload", () => {
        conn.invoke("UpdateUserStatus", userId, false);
      });
    };

    connect();

    return () => {
      if (connection) {
        connection.stop();
      }
    };
  }, [connection]);

  return (
    <SignalRContext.Provider value={{ userStatus }}>
      {children}
    </SignalRContext.Provider>
  );
};