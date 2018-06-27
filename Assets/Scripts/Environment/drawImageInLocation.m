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