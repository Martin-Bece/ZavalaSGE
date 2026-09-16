<template>
    <ion-tab-bar slot="bottom">
        <ion-tab-button
            v-for="item in items_tabs"
            :key="item.id"
            :tab="item.id"
            :href="item.ruta"
        >
            <ion-icon :icon="item.icono" />
            <ion-label>{{ item.titulo }}</ion-label>
        </ion-tab-button>
    </ion-tab-bar>
</template>

<script>
import {
    IonIcon,
    IonLabel,
    IonTabBar,
    IonTabButton
} from '@ionic/vue';
import { obtener_grupos_menu } from '@/config/navegacion';
import { use_sesion_store } from '@/stores/sesion_store';

export default {
    name: 'comp_tabs',
    components: {
        IonIcon,
        IonLabel,
        IonTabBar,
        IonTabButton
    },
    computed: {
        items_tabs() {
            const sesion_store = use_sesion_store();
            // Obtenemos los grupos filtrados por el rol actual
            const grupos = obtener_grupos_menu(sesion_store.rol_activo);
            
            // Aplanamos la estructura para que la barra de abajo los lea de corrido
            const todos_los_items = grupos.flatMap(grupo => grupo.items);

            // Tip: Si tu menú lateral tiene muchos ítems y abajo querés mostrar 
            // únicamente los primeros 4 o 5 principales, podés descomentar la siguiente línea:
            // return todos_los_items.slice(0, 4);

            return todos_los_items;
        }
    }
};
</script>