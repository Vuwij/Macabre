seed = 4;

r = rng(seed);
mapsize = 2^6+1;
M = createFractalTerrain(mapsize, 20, 0.99);

% Discretize z
Mint = int16(M);
heightMin = min(min(Mint));
heightMax = max(max(Mint));
Mint = Mint - heightMin;
totalHeight = heightMax - heightMin;

% plotMatrix3d(Mint);

tileset = '../../Spritesheets/Miscellaneous/NubblocksCube/';

[A1,~,transparency1] = imread(strcat(tileset,'1.png'));
[A2,~,transparency2] = imread(strcat(tileset,'2.png'));
[A3,~,transparency3] = imread(strcat(tileset,'3.png'));
[A4,~,transparency4] = imread(strcat(tileset,'4.png'));
[A5,~,transparency5] = imread(strcat(tileset,'5.png'));
[A6,~,transparency6] = imread(strcat(tileset,'6.png'));
[A7,~,transparency7] = imread(strcat(tileset,'7.png'));
[A8,~,transparency8] = imread(strcat(tileset,'8.png'));
[A9,~,transparency9] = imread(strcat(tileset,'9.png'));
[A10,~,transparency10] = imread(strcat(tileset,'10.png'));
[A11,~,transparency11] = imread(strcat(tileset,'11.png'));
[A12,~,transparency12] = imread(strcat(tileset,'12.png'));
[A13,~,transparency13] = imread(strcat(tileset,'13.png'));
[A14,~,transparency14] = imread(strcat(tileset,'14.png'));
[A15,~,transparency15] = imread(strcat(tileset,'15.png'));
[A16,~,transparency16] = imread(strcat(tileset,'16.png'));
[A17,~,transparency17] = imread(strcat(tileset,'17.png'));
[A18,~,transparency18] = imread(strcat(tileset,'18.png'));
[A19,~,transparency19] = imread(strcat(tileset,'19.png'));
[A20,~,transparency20] = imread(strcat(tileset,'20.png'));
[A21,~,transparency21] = imread(strcat(tileset,'21.png'));
[A22,~,transparency22] = imread(strcat(tileset,'22.png'));
[A23,~,transparency23] = imread(strcat(tileset,'23.png'));
[A24,~,transparency24] = imread(strcat(tileset,'24.png'));
[A25,~,transparency25] = imread(strcat(tileset,'25.png'));
[A26,~,transparency26] = imread(strcat(tileset,'26.png'));


Image1 = imghandle(A1, transparency1);
Image2 = imghandle(A2, transparency2);
Image3 = imghandle(A3, transparency3);
Image4 = imghandle(A4, transparency4);
Image5 = imghandle(A5, transparency5);
Image6 = imghandle(A6, transparency6);
Image7 = imghandle(A7, transparency7);
Image8 = imghandle(A8, transparency8);
Image9 = imghandle(A9, transparency9);
Image10 = imghandle(A10, transparency10);
Image11 = imghandle(A11, transparency11);
Image12 = imghandle(A12, transparency12);
Image13 = imghandle(A13, transparency13);
Image14 = imghandle(A14, transparency14);
Image15 = imghandle(A15, transparency15);
Image16 = imghandle(A16, transparency16);
Image17 = imghandle(A17, transparency17);
Image18 = imghandle(A18, transparency18);
Image19 = imghandle(A19, transparency19);
Image20 = imghandle(A20, transparency20);
Image21 = imghandle(A21, transparency21);
Image22 = imghandle(A22, transparency22);
Image23 = imghandle(A23, transparency23);
Image24 = imghandle(A24, transparency24);
Image25 = imghandle(A25, transparency25);
Image26 = imghandle(A26, transparency26);

l = length(A1);

blocksize = 4;

finishImageHeight = 17 + (mapsize - 2) * 8 + (totalHeight-1) * 8;
finishImageWidth = 17 + (mapsize - 2) * 4 * 2 * 2;
img = zeros(finishImageHeight, finishImageWidth, 3, 'uint8');
FinishImage = imghandle(img, 0);

for i = mapsize-1:-1:1
    for j = mapsize-1:-1:1
        h = Mint(i,j);
        for z = 1 : h
            if(z == h)
                dirN = 1;
                dirS = 1;
                dirE = 1;
                dirW = 1;
                if(i < mapsize-1)
                    dirW = Mint(i+1,j) < h;
                end
                if(i > 1)
                    dirE = Mint(i-1,j) < h;
                end
                if(j < mapsize-1)
                    dirN = Mint(i,j+1) < h;
                end
                if(j > 1)
                    dirS = Mint(i,j-1) < h;
                end
                if (dirN && dirW && dirE && dirS)
                    drawImageInLocation(i,j,z,Image20,FinishImage);
                elseif (dirN && dirW && dirE)
                    drawImageInLocation(i,j,z,Image26,FinishImage);
                elseif (dirN && dirW && dirS)
                    drawImageInLocation(i,j,z,Image23,FinishImage);
                elseif (dirN && dirE && dirS)
                    drawImageInLocation(i,j,z,Image26,FinishImage);
                elseif (dirW && dirE && dirS)
                    drawImageInLocation(i,j,z,Image24,FinishImage);
                elseif (dirN && dirS)
                    drawImageInLocation(i,j,z,Image4,FinishImage);
                elseif (dirE && dirW)
                    drawImageInLocation(i,j,z,Image2,FinishImage);
                elseif (dirN && dirE)
                    drawImageInLocation(i,j,z,Image8,FinishImage);
                elseif (dirN && dirW)
                    drawImageInLocation(i,j,z,Image10,FinishImage);
                elseif (dirS && dirE)
                    drawImageInLocation(i,j,z,Image1,FinishImage);
                elseif (dirS && dirW)
                    drawImageInLocation(i,j,z,Image12,FinishImage);
                elseif (dirN)
                    drawImageInLocation(i,j,z,Image9,FinishImage);
                elseif (dirS)
                    drawImageInLocation(i,j,z,Image4,FinishImage);
                elseif (dirE)
                    drawImageInLocation(i,j,z,Image2,FinishImage);
                elseif (dirW)
                    drawImageInLocation(i,j,z,Image11,FinishImage);
                else
                    drawImageInLocation(i,j,z,Image1,FinishImage);
                end
            else
                drawImageInLocation(i,j,z,Image1,FinishImage);
            end
        end
    end
end


h = imshow(FinishImage.img);

function drawImageInLocation(x, y, z, imgHandle, finishImageHandle)
    [l, w, ~] = size(finishImageHandle.img);
    [la, wa, ~] = size(imgHandle.img);
    
    xloc = ceil(l) - ((x-1) + (y-1)) * 4 - ceil(la) - (z-1) * 8;
    yloc = ceil(w/2) + ((x-1) - (y-1)) * 8 - ceil(wa/2);
   
    % Todo, better transparency
    for i = 1:la
        for j = 1:wa
            if(imgHandle.tran(i,j) == 0) 
                continue;
            end
            finishImageHandle.img(i+xloc,j+yloc,:) = imgHandle.img(i, j, :);
        end
    end
end


