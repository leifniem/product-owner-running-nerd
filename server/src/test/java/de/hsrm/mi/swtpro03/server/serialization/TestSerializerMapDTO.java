package de.hsrm.mi.swtpro03.server.serialization;

import de.hsrm.mi.swtpro03.server.game.dtos.EnemySpawnPointDTO;
import de.hsrm.mi.swtpro03.server.game.dtos.MapDTO;
import de.hsrm.mi.swtpro03.server.game.dtos.MapMetaDTO;
import de.hsrm.mi.swtpro03.server.game.model.Item;
import de.hsrm.mi.swtpro03.server.game.model.Map;
import de.hsrm.mi.swtpro03.server.game.model.Tile;
import de.hsrm.mi.swtpro03.server.game.model.Vector2;
import de.hsrm.mi.swtpro03.server.game.model.Vector2Int;
import de.hsrm.mi.swtpro03.server.utils.Serializer;
import org.junit.jupiter.api.AfterAll;
import org.junit.jupiter.api.BeforeAll;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;

class TestSerializerMapDTO {

    static MapDTO map;
    static MapMetaDTO meta;
    static Tile[][] tiles;
    static Item[][] items;

    @BeforeAll
    static void setUp() {
        meta = new MapMetaDTO("testName", 2);
        tiles = new Tile[meta.getNumberOfSections() * Map.SECTION_WIDTH][Map.SECTION_HEIGHT];
        tiles[10][12] = Tile.SOLID;
        tiles[12][15] = Tile.SOLID;
        items = new Item[meta.getNumberOfSections() * Map.SECTION_WIDTH][Map.SECTION_HEIGHT];
        items[38][6] = Item.PIZZA;
        items[10][13] = Item.CREDITPOINTS_5;
        items[12][15] = Item.CREDITPOINTS_5;
        items[39][6] = Item.CREDITPOINTS_5;
        map = new MapDTO(meta, tiles, items, new Vector2Int(0, 0), new EnemySpawnPointDTO[0]);
    }

    @Test
    @DisplayName("MapDTO should be serialized succesfully")
    void serializeMap() {
        String serializedMap = Serializer.serialize(map, MapDTO.class);
        assertNotNull(serializedMap);
    }

    @Test
    @DisplayName("MapDTO should be deserialized succesfully")
    void deserializeMap() {
        String serializedMap = Serializer.serialize(map, MapDTO.class);
        MapDTO deserializedMap = (MapDTO) Serializer.deserialize(serializedMap, MapDTO.class);
        assertNotNull(deserializedMap);
    }

    @Test
    @DisplayName("Deserialized MapDTO should be equal to original MapDTO")
    void checkEquality() {

        String serializedMap = Serializer.serialize(map, MapDTO.class);
        MapDTO deserializedMap = (MapDTO) Serializer.deserialize(serializedMap, MapDTO.class);
        assertEquals(map, deserializedMap);
        //EqualityCheck.CheckMapEquality(map, deserializedMap);
    }

    @AfterAll
    static void tearDown() {
        map = null;
        meta = null;
        tiles = null;
    }
}
