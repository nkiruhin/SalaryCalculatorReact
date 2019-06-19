import React from "react";
import { Dialog, DialogType, PrimaryButton, DefaultButton, DialogFooter } from "office-ui-fabric-react";


export interface IDeleteDialogProps {
    hidden: boolean;
    istrue(answer: boolean): void;
    text?: string;
    title?: string;
}

export const DeleteDialog: React.FC<IDeleteDialogProps> = ({ hidden, istrue, text, title }) => {
    const sayNo = (ev:any) => {
        istrue(false)
    }
    const sayYes = (ev:any) => {
        istrue(true)
    }
return  <div>
    <Dialog
        hidden={hidden}
        onDismiss={sayNo}
        dialogContentProps={{
            type: DialogType.normal,
            title:  title ,
            subText: text
        }}>
        <DialogFooter>
                <PrimaryButton onClick={sayYes} text="Да" />
                <DefaultButton onClick={sayNo} text="Нет" />
            </DialogFooter>
        </Dialog>
        </div>
}