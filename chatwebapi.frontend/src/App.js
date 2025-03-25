import { useState } from "react";
import { Container } from "react-bootstrap";
import StartPage from "./components/StartPage";
import ChatRoom from "./components/ChatRoom";
import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import "bootstrap/dist/css/bootstrap.min.css";

function App() {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [username, setUsername] = useState("");
  const [userId, setUserId] = useState("");
  const [messages, setMessages] = useState([]);
  const [connection, setConnection] = useState(null);
  const [currentChatId, setCurrentChatId] = useState(null);

  const onLoginSuccess = async (userName, userId) => {
    setIsAuthenticated(true);
    setUsername(userName);
    setUserId(userId);

    const conn = new HubConnectionBuilder()
      .withUrl("http://localhost:7027/chat", {
        accessTokenFactory: () => userId
      })
      .configureLogging(LogLevel.Information)
      .build();

    conn.on("ReceiveSpecificMessage", (username, msg, sentiment, chatId) => {
      setMessages(prev => [...prev, { username, msg, sentiment, chatId }]);
    });

    conn.on("JoinSpecificChatRoom", (username, msg, chatId) => {
      setMessages(prev => [...prev, { username, msg, chatId }]);
      setCurrentChatId(chatId);
    });

    try {
      await conn.start();
      setConnection(conn);
    } catch (error) {
      console.error("Connection failed:", error);
    }
  };

  const joinChat = async (chatName) => {
    if (connection) {
      try {
        await connection.invoke("JoinChatRoom", {
          RegisterModelId: userId,

          ChatName: chatName
        });
      } catch (error) {
        console.error("Join chat error:", error);
        throw error;
      }
    }
  };

  const sendMessage = async (message) => {
    const token = localStorage.getItem("token");
    if (connection && currentChatId) {
      try {
        await connection.invoke("SendMessageToGroup", currentChatId, message, token);
      } catch (error) {
        console.error("Send message error:", error);
      }
    }
  };

  return (
    <Container>
      {!isAuthenticated ? (
        <StartPage onLoginSuccess={onLoginSuccess} />
      ) : (
        <ChatRoom
          messages={messages.filter(msg => msg.chatId === currentChatId)}
          sendMessage={sendMessage}
          joinChat={joinChat}
          username={username}
          userId={userId}
          currentChatId={currentChatId}
          setCurrentChatId={setCurrentChatId}
        />
      )}
    </Container>
  );
}

export default App;