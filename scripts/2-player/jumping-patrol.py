flipped = False


def goLeft():
    InputState.leftKeyPressed = True
    InputState.rightKeyPressed = False
    InputState.jumpKeyPressed = False


def goRight():
    InputState.leftKeyPressed = False
    InputState.rightKeyPressed = True
    InputState.jumpKeyPressed = False


def doJump():
    InputState.jumpKeyPressed = True


def update():
    global flipped

    tiles = ProxyGameState.getTilesInRange()
    tileLeft = tiles[4][5]
    tileRightFloor = tiles[6][6]
    tileBeneath = tiles[5][6]

    if not flipped:
        if tileLeft.isLadder():
            flipped = True
        jump = not tileBeneath.isSolid()
    else:
        if tileRightFloor.isLadder():
            flipped = False
        jump = not tileRightFloor.isSolid()

    if flipped:
        goRight()
    else:
        goLeft()

    if jump:
        doJump()
