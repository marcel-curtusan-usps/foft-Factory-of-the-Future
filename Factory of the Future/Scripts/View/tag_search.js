let search_Table = $('table[id=tagresulttable]');
async function startSearch(sc) {
    try {
        let search = new RegExp(sc, 'i');
        let search_Table_Body = search_Table.find('tbody');
        search_Table_Body.empty();
        if (checkValue(sc)) {
            if (map.hasOwnProperty("_layers")) {
                $.map(map._layers, function (layer, i) {
                    if (layer.hasOwnProperty("feature")) {
                        if (/(person)|(Vehicle$)/i.test(layer.feature.properties.Tag_Type)) {
                            if (search.test(layer.feature.properties.name) || search.test(layer.feature.properties.id)) {
                                search_Table_Body.append(search_row_template.supplant(formatsearchrow(layer.feature.properties, layer._leaflet_id)));
                                if (layer.hasOwnProperty("options") && layer.options.hasOwnProperty("fillColor")) {

                                    if (!/red/i.test(layer.options.fillColor)) {
                                        layer.setStyle({
                                            fillColor: '#dc3545' //'red',
                                        });
                                    }
                                }
                                if (layer.hasOwnProperty("_tooltip") && layer._tooltip.hasOwnProperty("_container")) {

                                    if (layer._tooltip._container.classList.contains('tooltip-hidden')) {
                                        layer._tooltip._container.classList.remove('tooltip-hidden');
                                    }
                                    if (!layer._tooltip._container.classList.contains('searchflash')) {
                                        layer._tooltip._container.classList.add('searchflash');
                                    }
                                }
                            }
                            else {
                                if (!layer.feature.properties.hasOwnProperty("tagVisible")) {
                                    if (!layer._tooltip._container.classList.contains('tooltip-hidden')) {
                                        layer._tooltip._container.classList.add('tooltip-hidden');
                                    }
                                }
                                if (layer.hasOwnProperty("_tooltip")) {
                                    if (layer._tooltip.hasOwnProperty("_container")) {
                                        if (layer._tooltip._container.classList.contains('searchflash')) {
                                            layer._tooltip._container.classList.remove('searchflash');
                                        }
                                    }
                                }
                            }
                        }
                    }
                })
            }
        }
        else {
            if (map.hasOwnProperty("_layers")) {
                $.map(map._layers, function (layer, i) {
                    if (layer.hasOwnProperty("feature")) {
                        if (/(person)|(Vehicle$)/i.test(layer.feature.properties.Tag_Type)) {
                            if (layer.hasOwnProperty("_tooltip") && layer._tooltip.hasOwnProperty("_container")) {
                                if (layer._tooltip._container.classList.contains('searchflash')) {
                                    layer._tooltip._container.classList.remove('searchflash');
                                }
                                if (!layer.feature.properties.hasOwnProperty("tagVisible")) {
                                    if (!layer._tooltip._container.classList.contains('tooltip-hidden')) {
                                        layer._tooltip._container.classList.add('tooltip-hidden');
                                    }
                                }
                            }
                        }
                    }
                })
            }
        }
    } catch (e) {
     
    }
}
let search_row_template = '<tr data-id="{layer_id}" data-tag="{tag_id}">' +
    '<td><span class="ml-p25rem">{tag_name}</span></td>' +
    '<td class="align-middle text-center">' +
    '<button class="btn btn-light btn-sm mx-1 pi-iconEdit tagedit" name="tagedit"></button>' +
    '</td>' +
    '</tr>'
    ;
function formatsearchrow(properties, layer_id) {
    return $.extend(properties, {
        layer_id: layer_id,
        tag_id: properties.id,
        tag_name: checkValue(properties.name) ? properties.name : properties.id
    })
}
