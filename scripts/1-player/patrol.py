flipped = False


def goLeft():
    InputState.leftKeyPressed = True
    InputState.rightKeyPressed = False


def goRight():
    InputState.leftKeyPressed = False
    InputState.rightKeyPressed = True


def update():
    global flipped

    tiles = ProxyGameState.getTilesInRange()
    tileLeft = tiles[4][5]
    tileRight = tiles[6][5]

    if not flipped:
        if tileLeft.isLadder():
            flipped = True
    else:
        if tileRight.isSolid():
            flipped = False

    if flipped:
        goRight()
    else:
        goLeft()
