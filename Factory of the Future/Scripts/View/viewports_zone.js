﻿/*
 this is for view ports
 */
$(function () {
    $.extend(fotfmanager.client, {
        updateViewPortStatus: async (viewportzoneupdate) => { updateviewportzone(viewportzoneupdate)      }
    });
    
});
var viewPortsAreas = new L.GeoJSON(null, {
    style: function (feature) {
        return {
            weight: 2,
            opacity: 1,
            color: '#3573b1',
            fillColor: 'pink',
            fillOpacity: 0.2,
            label: feature.properties.name
        };
    },
    onEachFeature: function (feature, layer) {
        layer.zoneId = feature.properties.id;
        var vp_name = feature.properties.name;
        var vp_namer = vp_name.replace(/^VP_/, '');
        $('<div/>', { id: 'div_' + feature.properties.id }).append($('<button/>', { class: 'btn btn-light btn-sm mx-1 py-2 viewportszones', id: feature.properties.id, text: vp_namer })).appendTo($('div[id=viewportsContent]'))
        layer.bindTooltip(feature.properties.name, {
            permanent: true,
            interactive: true,
            direction: 'center',
            opacity: 1
        }).openTooltip();
        layer.bringToBack();
    }
});