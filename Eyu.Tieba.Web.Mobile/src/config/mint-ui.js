import {
  Button,
  Cell,
  Field,
  Header,
  Indicator,
  MessageBox,
  Popup,
  // Spinner,
  Tabbar,
  TabContainer,
  TabContainerItem,
  TabItem,
  Toast
} from 'mint-ui'

export default {
  install (Vue) {
    Vue.component(Button.name, Button);
    Vue.component(Cell.name, Cell);
    Vue.component(Field.name, Field);
    Vue.component(Header.name, Header);
    Vue.component(Popup.name, Popup);
    // Vue.component(Spinner.name, Spinner);
    Vue.component(Tabbar.name, Tabbar);
    Vue.component(TabContainer.name, TabContainer);
    Vue.component(TabContainerItem.name, TabContainerItem);
    Vue.component(TabItem.name, TabItem);

    Vue.prototype.$toast = Toast;
    Vue.prototype.$confirm = MessageBox.confirm;
    Vue.prototype.$loading = Indicator;
  }
}
