import { useState } from "react"
import { Button, Form, InputGroup } from "react-bootstrap"

const SendMessageForm = ({sendMessage}) => {
    const[msg, setMessages] = useState('');

    return <Form onSubmit={e => {
        e.preventDefault();
        sendMessage(msg);
        setMessages('');
    }}>
        <InputGroup className="mb-3">
            <InputGroup.Text>Chat</InputGroup.Text>
            <Form.Control onChange={e => setMessages(e.target.value)} value={msg} placeholder="type message"></Form.Control>
            <Button variant="primary" type="submit" disabled={!msg}>Send</Button>
        </InputGroup>
    </Form>
}

export default SendMessageForm;