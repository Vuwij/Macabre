classdef imghandle < handle
    %IMGHANDLE Summary of this class goes here
    %   Detailed explanation goes here
    
    properties
        img
        tran
    end
    
    methods
        function obj = imghandle(img,tran)
            %IMGHANDLE Construct an instance of this class
            %   Detailed explanation goes here
            obj.img = img;
            obj.tran = tran;
        end
    end
end

