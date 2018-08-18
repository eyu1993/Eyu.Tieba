import {
  Button,
  Card,
  Col,
  Collapse,
  CollapseItem,
  Dialog,
  Form,
  FormItem,
  Input,
  Loading,
  Menu,
  MenuItem,
  Message,
  MessageBox,
  Row,
  Submenu,
  Tabs,
  TabPane,
  Table,
  TableColumn
} from 'element-ui'

export default {
  install (Vue) {
    Vue.use(Button)
    Vue.use(Card)
    Vue.use(Col)
    Vue.use(Collapse)
    Vue.use(CollapseItem)
    Vue.use(Dialog)
    Vue.use(Form)
    Vue.use(FormItem)
    Vue.use(Input)
    Vue.use(Loading.directive)
    Vue.use(Menu)
    Vue.use(MenuItem)
    Vue.use(Row)
    Vue.use(Submenu)
    Vue.use(Tabs)
    Vue.use(TabPane)
    Vue.use(Table)
    Vue.use(TableColumn)

    Vue.prototype.$loading = Loading.service
    Vue.prototype.$message = Message
    Vue.prototype.$confirm = MessageBox.confirm
  }
}
