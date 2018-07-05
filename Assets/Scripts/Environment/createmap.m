seed = 4;

r = rng(seed);
<<<<<<< HEAD
mapsize = 2^6+1;

%so this is where I work
% M = createFractalTerrain(mapsize, 20, 0.99);

%--FLATMAP--
M = 1 .* ones(mapsize,mapsize);
M(1,1) = 0;

%--SLOPES?--
% maxheight = 10;
% 
% for xloc = 1:1:mapsize
%     for yloc = 1:1:mapsize
%         M(xloc,yloc) = (( log(xloc) + log(yloc))*maxheight)/(2*log(mapsize)) + log((M(xloc,yloc))+2);
%     end
% end
randheight = 1;    
Maxblocksize = 10;
concentration = mapsize/2;

for count = 1:(mapsize)^2
    
    count
    
    randlocx = abs(floor(normrnd(concentration, 12)))+1;
    randlocy = abs(floor(normrnd(concentration, 12)))+1;
    
    
    while (randlocx >= mapsize-Maxblocksize)
        randlocx = abs(floor((normrnd(concentration, 12))));
    end
    
    while (randlocy >= mapsize-Maxblocksize)
        randlocy = abs(floor((normrnd(concentration, 12))));
    end
    
    blocksize = floor((rand*6)+12);
    
    randheight = (rand*5+2);
    
    heightsample = floor(M(randlocx,randlocy) + randheight);
    
    for blockx  = 1:blocksize
        for blocky = 1:blocksize
            
            x = randlocx + blockx;
            y = randlocy + blocky;
            
                M(x,y) = heightsample;
            
        end
    end
end

%so this is where I work
=======
mapsize = 2^7+1;
M = createFractalTerrain(mapsize, 20, 0.99);
% M = log((M+10))/log(1.15);
% M = 20*real(1.7.^(M))./(1+real(1.7.^(M)));
% M = sin(M./3)*10; % Craters
M = 1.4.^(M); % Mountains
% M = ones(mapsize, mapsize);
>>>>>>> master

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
[A27,~,transparency27] = imread(strcat(tileset,'27.png'));
[A28,~,transparency28] = imread(strcat(tileset,'28.png'));
[A29,~,transparency29] = imread(strcat(tileset,'29.png'));
[A30,~,transparency30] = imread(strcat(tileset,'30.png'));
[A31,~,transparency31] = imread(strcat(tileset,'31.png'));
[A32,~,transparency32] = imread(strcat(tileset,'32.png'));
[A33,~,transparency33] = imread(strcat(tileset,'33.png'));
[A34,~,transparency34] = imread(strcat(tileset,'34.png'));
[A35,~,transparency35] = imread(strcat(tileset,'35.png'));
[A36,~,transparency36] = imread(strcat(tileset,'36.png'));

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
Image27 = imghandle(A27, transparency27);
Image28 = imghandle(A28, transparency28);
Image29 = imghandle(A29, transparency29);
Image30 = imghandle(A30, transparency30);
Image31 = imghandle(A31, transparency31);
Image32 = imghandle(A32, transparency32);
Image33 = imghandle(A33, transparency33);
Image34 = imghandle(A34, transparency34);
Image35 = imghandle(A35, transparency34);
Image36 = imghandle(A36, transparency34);

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
                    drawImageInLocation(i,j,z,Image35,FinishImage);
                elseif (dirN && dirW && dirE)
                    drawImageInLocation(i,j,z,Image28,FinishImage);
                elseif (dirN && dirW && dirS)
                    drawImageInLocation(i,j,z,Image30,FinishImage);
                elseif (dirN && dirE && dirS)
                    drawImageInLocation(i,j,z,Image26,FinishImage);
                elseif (dirW && dirE && dirS)
                    drawImageInLocation(i,j,z,Image32,FinishImage);
                elseif (dirN && dirS)
                    drawImageInLocation(i,j,z,Image34,FinishImage);
                elseif (dirE && dirW)
                    drawImageInLocation(i,j,z,Image32,FinishImage);
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



