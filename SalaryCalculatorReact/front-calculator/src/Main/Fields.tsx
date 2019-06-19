import * as React from 'react'
import { TextField, MaskedTextField } from "office-ui-fabric-react/lib/TextField";
import { DatePicker, DayOfWeek, IDatePickerStrings } from 'office-ui-fabric-react/lib/DatePicker';
import { ComboBox } from 'office-ui-fabric-react/lib/ComboBox';
import fetchApi from '../fetcher';
import { isDate } from 'util';
import { Checkbox } from 'office-ui-fabric-react';

const FieldStyle = {
    width: '500px'
};

const DatePickerStrings: IDatePickerStrings = {
    months: ['Январь', 'Февраль', 'Март', 'Апрель', 'Май', 'Июнь', 'Июль', 'Август', 'Сентябрь', 'Октябрь', 'Ноябрь', 'Декабрь'],

    shortMonths: ['Янв', 'Фев', 'Мар', 'Апр', 'Май', 'Июн', 'Июл', 'Авг', 'Сен', 'Окт', 'Ноя', 'Дек'],

    days: ['Воскресенье', 'Понедельник', 'Вторник', 'Среда', 'Четверг', 'Пятница', 'Суббота'],

    shortDays: ['Вc', 'Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб'],

    goToToday: 'Сегодня',
    prevMonthAriaLabel: 'К предидущему месяцу',
    nextMonthAriaLabel: 'К следующему месяцу',
    prevYearAriaLabel: 'К предидущему году',
    nextYearAriaLabel: 'К следующему году',
    closeButtonAriaLabel: 'Закрыть',
    invalidInputErrorMessage: 'Не верный формат даты'
};

const _onFormatDate = (date?: Date): string => {
    if (date != null) return date.toLocaleDateString('ru-RU')
    return ''
}
const _parseDate = (dateStr: string): Date => {
    var dateParts = dateStr.split(".");
    return new Date(+dateParts[2], (+dateParts[1] - 1), +dateParts[0]);
}


export const Field: React.FC<any> = (props: any) => {

    const _items = () => {
        if (props.items !== null && !props.required) props.items.unshift({ key: '', text: '' })
        return props.items
    }
    const [items, setItems] = React.useState(_items)
    const [loading, setLoading] = React.useState(false)
    const _onGetErrorMessage = (value: string) => (
        value === '' && props.required ? "поле обязательно" + value : ''
    )
    const _renderField = () => {
        switch (props.type) {
            case "DateTime":
                return <DatePicker
                    id={props.id}
                    label={props.label}
                    style={FieldStyle}
                    allowTextInput={true}
                    parseDateFromString= { _parseDate }
                    isRequired={props.required}
                    strings={DatePickerStrings}
                    showMonthPickerAsOverlay={true}
                    firstDayOfWeek={DayOfWeek.Sunday}
                    onSelectDate={_handleChange}
                    formatDate={_onFormatDate}
                    disabled={props.edit ? false : true}
                    value={props.value === '' ? undefined : new Date(props.value)}
                />
            case "select":
                if (props.serverSide && !loading) {
                    _getItemsFromServer();
                }

                return <ComboBox
                    style={FieldStyle}
                    id={props.id}
                    label={props.label}
                    required={props.required}
                    options={items}
                    onChange={_handleChange}
                    selectedKey={props.required && props.value === '' ? items["0"].key : + props.value}
                    useComboBoxAsMenuWidth={true}
                    disabled={props.edit ? false : true}
                />
                    
            case "Single":
                return <MaskedTextField
                    id={props.id}
                    name={props.name}
                    label={props.label}
                    style={FieldStyle}
                    mask="99999.99"
                    required={props.required}
                    onChange={_handleChange}
                    onGetErrorMessage={_onGetErrorMessage}
                    disabled={props.edit ? false : true}
                    value={props.value === '' ? undefined : props.value} />                   
            case "Boolean":
                return <Checkbox
                    styles={{ root: { width: '500px', paddingTop:'30px' } }}
                    id={props.id}
                    label={props.label}
                    name={props.name}
                    defaultChecked={props.value === '' || props.value.toLowerCase() === 'false' ? false : true}
                    onChange={_handleChange}
                    onErrorCapture={() => (console.log("OnError"))}
                    disabled={props.edit ? false : true}
                />
            default:
                return <TextField
                    id={props.id}
                    name={props.name}
                    label={props.label}
                    style={FieldStyle}
                    required={props.required}
                    type={props.type}
                    onChange={_handleChange}
                    onGetErrorMessage = {_onGetErrorMessage}
                    value={props.value === '' ? undefined : props.value}
                    disabled={props.edit ? false : true}
                />
        }
    }
    const _getItemsFromServer = () => {
        const url = "api/" + props.formName + "/" + props.name
        setLoading(true);
        fetchApi(setItems, url,"GET");
    }
    const _handleChange = (evn: any, newValue?: any, index?: any) => {
        
        if (props.type === "Single") {
            props.onChange(props.name, +newValue);
            return;
        }
        if (props.type === "Boolean") {
            props.onChange(props.name, newValue);
            return;
        }
        if (index > -1) {
            props.onChange(props.name, newValue.key);
            return;
        }
        if (newValue) {
            props.onChange(props.name,newValue);
            return;
        }
        if (isDate(evn)) {
            props.onChange(props.name, evn.toISOString());
            return;
        }
    }
    return (
        <div>
            { _renderField() }
        </div>
    )
}