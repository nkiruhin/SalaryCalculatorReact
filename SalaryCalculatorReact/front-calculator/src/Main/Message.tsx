import React from 'react';
import { MessageBarType, MessageBar } from 'office-ui-fabric-react';

export interface IMessage {
    messageType?: MessageBarType;
    message?: string;
    show: boolean;
}
export const Message: React.FC<IMessage> = ({ message, messageType, show }) => {
    return (show) ? <MessageBar messageBarType={messageType} isMultiline={false} onDismiss={() => (null)} dismissButtonAriaLabel="Close" >
        {message}
    </MessageBar> : null
}