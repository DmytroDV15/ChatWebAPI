const MessageContainer = ({ messages = [] }) => {
    return (
        <div>
            {messages.map((msg, index) => {
                let color = "black";
                debugger
                if (msg.sentiment === "Positive") color = "green";
                else if (msg.sentiment === "Negative") color = "red";
                else if (msg.sentiment === "Neutral") color = "black";

                return (
                    <div key={index} style={{ color: color, margin: "5px 0" }}>
                        <strong>{msg.username}:</strong> {msg.msg}
                    </div>
                );
            })}
        </div>
    );
};

export default MessageContainer;
