isWalkingLeft = False


def goLeft():
    InputState.leftKeyPressed = True
    InputState.rightKeyPressed = False


def goRight():
    InputState.leftKeyPressed = False
    InputState.rightKeyPressed = True


def directionIsWalkable(tile):
    return tile.isSolid() or tile.isLadder()


def guardArea(tiles):
    global isWalkingLeft

    leftFloorTile = tiles[4][6]
    rightFloorTile = tiles[6][6]

    if not isWalkingLeft:
        if not directionIsWalkable(rightFloorTile):
            isWalkingLeft = True
    else:
        if not directionIsWalkable(leftFloorTile):
            isWalkingLeft = False

    if isWalkingLeft:
        goLeft()
    else:
        goRight()


def update():
    tiles = ProxyGameState.getTilesInRange()
    guardArea(tiles)
