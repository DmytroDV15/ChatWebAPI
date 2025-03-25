import { useState, useEffect } from "react";
import { Row, Col, Button, Form, ListGroup } from "react-bootstrap";
import MessageContainer from "./MessageContainer";
import SendMessageForm from "./SendMessages";

const ChatRoom = ({
  messages,
  sendMessage,
  username,
  userId,
  joinChat,
  currentChatId,
  setCurrentChatId,
}) => {
  const [chatName, setChatName] = useState("");
  const [error, setError] = useState("");
  const [userChats, setUserChats] = useState([]);

  useEffect(() => {
    const fetchChats = async () => {
      try {
        const response = await fetch(`http://localhost:7027/api/userschats/search?id=${userId}`);
        if (!response.ok) {
          throw new Error(`Failed to fetch chats: ${response.status}`);
        }
        const data = await response.json();
        const chats = data["$values"];

        const chatsWithIds = chats.map((chatName, index) => ({
          id: index + 1,
          chatName,
        }));

        setUserChats(chatsWithIds);
      } catch (err) {
        console.error("Failed to fetch user chats:", err);
        setError("Failed to load your chats. Please try again later.");
      }
    };

    if (userId) {
      fetchChats();
    }
  }, [userId]);

  const handleChatAction = async (action) => {
    if (chatName.trim() === "") {
      setError("Chat name cannot be empty.");
      return;
    }

    try {
      const payload = {
        RegisterModelId: userId,
        ChatName: chatName,
      };

      const response = await fetch(`http://localhost:7027/api/chat/${action}`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(payload),
      });

      if (!response.ok) {
        throw new Error(`Failed to ${action} chat: ${response.status}`);
      }

      const chatRoom = await response.json();
      setUserChats((prevChats) => {
        if (!prevChats.some((chat) => chat.id === chatRoom.id)) {
          return [...prevChats, { id: chatRoom.id, chatName: chatRoom.chatName }];
        }
        return prevChats;
      });
      setChatName("");
      setError("");

      await joinChat(chatRoom.chatName);
      setCurrentChatId(chatRoom.id);  

    } catch (err) {
      console.error("Error in handleChatAction:", err);
      setError(
        action === "create"
          ? "Failed to create chatroom. Please try again."
          : "Chatroom does not exist or cannot be joined."
      );
    }
  };

  const handleChatSelection = async (chat) => {
    try {
      const payload = {
        RegisterModelId: userId,
        ChatName: chat.chatName,
        
      };
  
      const response = await fetch("http://localhost:7027/api/chat/join", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(payload),
      });
  
      if (!response.ok) {
        throw new Error("Failed to join chat.");
      }
  
      const chatRoom = await response.json();
      setCurrentChatId(chatRoom.id);
  
      // Join chatroom via SignalR after fetch succeeds
      await joinChat(chat.chatName); 
      setError("");
    } catch (err) {
      console.error("Failed to join chat:", err);
      setError("Unable to join the selected chat.");
    }
  };
  
  return (
    <div className="d-flex flex-column h-100">
      <Row className="flex-grow-1">
        <Col sm={3} className="border-end">
          <h1>{username}</h1>
          <h5 className="py-3">Your Chats</h5>
          <Form className="mb-3">
            <Form.Group>
              <Form.Control
                type="text"
                placeholder="Enter chatroom name"
                value={chatName}
                onChange={(e) => setChatName(e.target.value)}
                required
              />
            </Form.Group>
            <div className="mt-2">
              <Button
                variant="primary"
                className="me-2"
                onClick={() => handleChatAction("create")}
              >
                Create
              </Button>
              <Button
                variant="success"
                onClick={() => handleChatAction("join")}
              >
                Join
              </Button>
            </div>
          </Form>
          <ListGroup className="overflow-auto" style={{ maxHeight: "calc(100vh - 200px)" }}>
            {userChats.length > 0 ? (
              userChats.map((chat) => (
                <ListGroup.Item
                  key={chat.id}
                  active={chat.id === currentChatId}
                  action
                  onClick={() => handleChatSelection(chat)}
                >
                  {chat.chatName}
                </ListGroup.Item>
              ))
            ) : (
              <p>No chats available.</p>
            )}
          </ListGroup>
        </Col>

        <Col sm={9} className="d-flex flex-column">
          <div className="py-3">
            <h2 className="text-center">
              {currentChatId
                ? `Chatroom: ${userChats.find(chat => chat.id === currentChatId)?.chatName || "Unknown Chat"}`
                : "No Chatroom Selected"}
            </h2>
            {error && <p className="text-danger text-center">{error}</p>}
          </div>

          <div className="flex-grow-1 overflow-auto mb-4">
            {currentChatId && <MessageContainer messages={messages} />}
          </div>
          {currentChatId && (
            <div className="mt-3">
              <SendMessageForm sendMessage={sendMessage} />
            </div>
          )}
        </Col>
      </Row>
    </div>
  );
};

export default ChatRoom;
